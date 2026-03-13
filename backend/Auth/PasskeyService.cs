using System.Security.Cryptography;
using backend.Auth;
using backend.DataAccess;
using backend.DataAccess.Models;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

public class PasskeyService : IPasskeyService
{
    private readonly IFido2 _fido2;
    private readonly TytDbContext _db;
    private readonly IDistributedCache _cache;

    public PasskeyService(IFido2 fido2, TytDbContext db, IDistributedCache cache)
    {
        _fido2 = fido2;
        _db = db;
        _cache = cache;
    }


    public async Task<CredentialCreateOptions> BeginRegistrationAsync(string username)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            user = new AppUser
            {
                Id = Guid.NewGuid(),
                Username = username,
                DisplayName = username,
                UserHandle = RandomNumberGenerator.GetBytes(32)
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        // Vorhandene Credentials ausschließen (kein Doppel-Registrieren)
        var existingCredentials = await _db.PasskeyCredentials
            .Where(c => c.UserId == user.Id)
            .Select(c => new PublicKeyCredentialDescriptor(c.Id))
            .ToListAsync();

        var fidoUser = new Fido2User
        {
            Id = user.UserHandle,
            Name = user.Username,
            DisplayName = user.DisplayName
        };

        var options = _fido2.RequestNewCredential(
            new RequestNewCredentialParams
            {
                User = fidoUser,
                ExcludeCredentials = existingCredentials,
                AuthenticatorSelection = new AuthenticatorSelection
                {
                    ResidentKey = ResidentKeyRequirement.Required,     // für Passkeys wichtig
                    UserVerification = UserVerificationRequirement.Required
                },
                AttestationPreference = AttestationConveyancePreference.None
            }

        );

        // Challenge in Cache speichern (2 Minuten TTL)
        await _cache.SetStringAsync(
            $"passkey:reg:{username}",
            options.ToJson(),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) }
        );

        return options;
    }

    public async Task<string> CompleteRegistrationAsync(
        string username,
        AuthenticatorAttestationRawResponse attestationResponse)
    {
        var optionsJson = await _cache.GetStringAsync($"passkey:reg:{username}")
            ?? throw new Exception("Registrierungs-Session abgelaufen.");

        var options = CredentialCreateOptions.FromJson(optionsJson);
        await _cache.RemoveAsync($"passkey:reg:{username}");

        // Callback: Ist diese Credential-ID bereits in Verwendung?
        IsCredentialIdUniqueToUserAsyncDelegate uniqueCheck = async (args, _) =>
            !await _db.PasskeyCredentials.AnyAsync(c => c.Id.SequenceEqual(args.CredentialId));

        var result = await _fido2.MakeNewCredentialAsync(
            new MakeNewCredentialParams
            {
                AttestationResponse = attestationResponse,
                OriginalOptions = options,
                IsCredentialIdUniqueToUserCallback = uniqueCheck
            });

        var user = await _db.Users.FirstAsync(u => u.Username == username);

        _db.PasskeyCredentials.Add(new PasskeyCredential
        {
            Id = result.Id,
            UserId = user.Id,
            PublicKey = result.PublicKey,
            UserHandle = result.User.Id,
            SignCount = result.SignCount,
            Transports = result.Transports?.Select(t => t.ToString()).ToArray() ?? [],
            IsBackupEligible = result.IsBackupEligible,
            IsBackedUp = result.IsBackedUp,
            AttestationFormat = result.AttestationFormat,
        });

        await _db.SaveChangesAsync();
        return await CreateSessionAsync(user.Id);
    }



    public async Task<AssertionOptions> BeginLoginAsync(string? username)
    {
        List<PublicKeyCredentialDescriptor> allowedCredentials = new();

        if (username != null)
        {
            var user = await _db.Users.Include(u => u.Credentials)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user != null)
                allowedCredentials = user.Credentials
                    .Select(c => new PublicKeyCredentialDescriptor(c.Id))
                    .ToList();
        }
        // Bei leerem allowedCredentials: Passkey wählt automatisch (discoverable)

        var options = _fido2.GetAssertionOptions(
            new GetAssertionOptionsParams
            {
                AllowedCredentials = allowedCredentials,
                UserVerification = UserVerificationRequirement.Required
            }

        );

        var cacheKey = $"passkey:login:{options.Challenge}";
        await _cache.SetStringAsync(
            cacheKey,
            options.ToJson(),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) }
        );

        return options;
    }

    public async Task<string> CompleteLoginAsync(AuthenticatorAssertionRawResponse assertionResponse)
    {
        // Challenge aus der Response lesen, um Cache-Eintrag zu finden
        var challenge = assertionResponse.Response.ClientDataJson; // wird intern geprüft

        // Wir speichern die Challenge-ID im Cache-Key – hol alle möglichen oder nutze separate ID
        // Einfachste Lösung: Challenge-Key wird beim Begin-Request als Cookie/Header zurückgegeben
        // Hier: wir nutzen die RawId der Credential zum Nachschlagen
        var credentialId = WebEncoders.Base64UrlDecode(assertionResponse.Id);

        var credential = await _db.PasskeyCredentials
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id.SequenceEqual(credentialId))
            ?? throw new Exception("Unbekannte Credential.");

        // Challenge aus Cache (Key wurde als Header mitgesendet – siehe Controller)
        var optionsJson = await _cache.GetStringAsync($"passkey:login:{challenge}")
            ?? throw new Exception("Login-Session abgelaufen.");

        var options = AssertionOptions.FromJson(optionsJson);
        await _cache.RemoveAsync($"passkey:login:{challenge}");

        var result = await _fido2.MakeAssertionAsync(
            new MakeAssertionParams
            {
                AssertionResponse = assertionResponse,
                OriginalOptions = options,
                StoredPublicKey = credential.PublicKey,
                StoredSignatureCounter = credential.SignCount,
                IsUserHandleOwnerOfCredentialIdCallback = (args, _) =>
                    Task.FromResult(credential.UserHandle.SequenceEqual(args.UserHandle))
            }
        );

        // SignCount aktualisieren (Replay-Schutz!)
        credential.SignCount = result.SignCount;
        credential.LastUsedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
       
        return await CreateSessionAsync(credential.User.Id);
    }

    private async Task<string> CreateSessionAsync(Guid userId)
    {
        var sessionId = Guid.NewGuid().ToString();

        await _cache.SetStringAsync(
            $"session:{sessionId}",
            userId.ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(3),
                SlidingExpiration = TimeSpan.FromMinutes(30)
            }
        );

        return sessionId;
    }

    public async Task<AppUser?> GetUserBySessionAsync(string sessionId)
    {
        var userIdStr = await _cache.GetStringAsync($"session:{sessionId}");
        if (userIdStr == null) return null;

        var userId = Guid.Parse(userIdStr);

        return await _db.Users.FindAsync(userId);
    }
}