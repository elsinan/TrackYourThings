

using System.Text.Json;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Mvc;
using backend.DataAccess.Models;
using System.Text;


namespace backend.Auth;

[ApiController]
[Route("api/passkey")]
public class PasskeyController(IPasskeyService passkeyService) : ControllerBase
{
    private readonly IFido2? _fido2;
    private readonly IPasskeyService _passkeyService = passkeyService ; 

  private string FormatException(Exception e)
    {
        return string.Format("{0}{1}", e.Message, e.InnerException != null ? " (" + e.InnerException.Message + ")" : "");
    }

    [HttpPost]
    [Route("/makeCredentialOptions")]
    public async Task<JsonResult> MakeCredentialOptionsAsync([FromForm] string username,
                                            [FromForm] string displayName,
                                            [FromForm] string attType,
                                            [FromForm] string authType,
                                            [FromForm] string residentKey,
                                            [FromForm] string userVerification)
    {
        try
        {

            if (string.IsNullOrEmpty(username))
            {
                username = $"{displayName} (Usernameless user created at {DateTime.UtcNow})";
            }

            // 1. Get user from DB by username (in our example, auto create missing users)
            var user = await _passkeyService.GetOrAddUser(username, () => new Fido2User
            {
                DisplayName = displayName,
                Name = username,
                Id = Encoding.UTF8.GetBytes(username) // byte representation of userID is required
            });

            // 2. Get user existing keys by username
            var existingKeys = (await _passkeyService.GetCredentialsByUser(user)).Select(c => c.Descriptor).ToList();

            // 3. Create options
            var authenticatorSelection = new AuthenticatorSelection
            {
                ResidentKey = residentKey.ToEnum<ResidentKeyRequirement>(),
                UserVerification = userVerification.ToEnum<UserVerificationRequirement>()
            };

            if (!string.IsNullOrEmpty(authType))
                authenticatorSelection.AuthenticatorAttachment = authType.ToEnum<AuthenticatorAttachment>();

            var exts = new AuthenticationExtensionsClientInputs()
            {
                Extensions = true,
                UserVerificationMethod = true,
                CredProps = true
            };

            var options = _fido2.RequestNewCredential(new RequestNewCredentialParams { User = user, ExcludeCredentials = existingKeys, AuthenticatorSelection = authenticatorSelection, AttestationPreference = attType.ToEnum<AttestationConveyancePreference>(), Extensions = exts });

            // 4. Temporarily store options, session/in-memory cache/redis/db
            HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());

            // 5. return options to client
            return new JsonResult(options);
        }
        catch (Exception e)
        {
            return new JsonResult(new { Status = "error", ErrorMessage = FormatException(e) });
        }
    }

    [HttpPost]
    [Route("/makeCredential")]
    public async Task<JsonResult> MakeCredential([FromBody] AuthenticatorAttestationRawResponse attestationResponse, CancellationToken cancellationToken)
    {
        try
        {
            // 1. get the options we sent the client
            var jsonOptions = HttpContext.Session.GetString("fido2.attestationOptions")!;
            var options = CredentialCreateOptions.FromJson(jsonOptions);

            // 2. Create callback so that lib can verify credential id is unique to this user
            async Task<bool> callback(IsCredentialIdUniqueToUserParams args, CancellationToken cancellationToken)
            {
                var users = await _passkeyService.GetUsersByCredentialIdAsync(args.CredentialId, cancellationToken);
                if (users.Count > 0)
                    return false;

                return true;
            }

            // 2. Verify and make the credentials
            var credential = await _fido2.MakeNewCredentialAsync(new MakeNewCredentialParams
            {
                AttestationResponse = attestationResponse,
                OriginalOptions = options,
                IsCredentialIdUniqueToUserCallback = callback
            }, cancellationToken: cancellationToken);

            // 3. Store the credentials in db
            await _passkeyService.AddCredentialToUser(options.User, new StoredCredential
            {
                CredentialId = credential.Id,
                PublicKey = credential.PublicKey,
                UserHandle = credential.User.Id,
                SignCount = credential.SignCount,
                AttestationFormat = credential.AttestationFormat,
                RegDate = DateTimeOffset.UtcNow,
                AaGuid = credential.AaGuid,
                Transports = credential.Transports,
                IsBackupEligible = credential.IsBackupEligible,
                IsBackedUp = credential.IsBackedUp,
                AttestationObject = credential.AttestationObject,
                AttestationClientDataJson = credential.AttestationClientDataJson
            });

            // 4. return "ok" to the client
            return new JsonResult(credential);
        }
        catch (Exception e)
        {
            return new JsonResult(new { status = "error", errorMessage = FormatException(e) });
        }
    }

    [HttpPost]
    [Route("/assertionOptions")]
    public async Task<ActionResult> AssertionOptionsPostAsync([FromForm] string username, [FromForm] string userVerification)
    {
        try
        {
            List<PublicKeyCredentialDescriptor> existingCredentials = [];

            if (!string.IsNullOrEmpty(username))
            {
                // 1. Get user from DB
                var user = await _passkeyService.GetUser(username) ?? throw new ArgumentException("Username was not registered");

                // 2. Get registered credentials from database
                existingCredentials = (await _passkeyService.GetCredentialsByUser(user)).Select(c => c.Descriptor).ToList();
            }

            var exts = new AuthenticationExtensionsClientInputs()
            {
                Extensions = true,
                UserVerificationMethod = true
            };

            // 3. Create options
            var uv = string.IsNullOrEmpty(userVerification) ? UserVerificationRequirement.Discouraged : userVerification.ToEnum<UserVerificationRequirement>();
            var options = _fido2.GetAssertionOptions(new GetAssertionOptionsParams()
            {
                AllowedCredentials = existingCredentials,
                UserVerification = uv,
                Extensions = exts
            });

            // 4. Temporarily store options, session/in-memory cache/redis/db
            HttpContext.Session.SetString("fido2.assertionOptions", options.ToJson());

            // 5. Return options to client
            return new JsonResult(options);
        }

        catch (Exception e)
        {
            return new JsonResult(new { Status = "error", ErrorMessage = FormatException(e) });
        }
    }

    [HttpPost]
    [Route("/makeAssertion")]
    public async Task<JsonResult> MakeAssertion([FromBody] AuthenticatorAssertionRawResponse clientResponse, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get the assertion options we sent the client
            var jsonOptions = HttpContext.Session.GetString("fido2.assertionOptions")!;
            var options = AssertionOptions.FromJson(jsonOptions);

            // 2. Get registered credential from database
            var creds = await _passkeyService.GetCredentialById(clientResponse.RawId) ?? throw new Exception("Unknown credentials");

            // 3. Get credential counter from database
            var storedCounter = creds.SignCount;

            // 4. Create callback to check if the user handle owns the credentialId
            async Task<bool> callback(IsUserHandleOwnerOfCredentialIdParams args, CancellationToken cancellationToken)
            {
                var storedCreds = await _passkeyService.GetCredentialsByUserHandleAsync(args.UserHandle, cancellationToken);
                return storedCreds.Exists(c => c.Descriptor.Id.SequenceEqual(args.CredentialId));
            }

            // 5. Make the assertion
            var res = await _fido2!.MakeAssertionAsync(new MakeAssertionParams
            {
                AssertionResponse = clientResponse,
                OriginalOptions = options,
                StoredPublicKey = creds.PublicKey,
                StoredSignatureCounter = storedCounter,
                IsUserHandleOwnerOfCredentialIdCallback = callback
            }, cancellationToken: cancellationToken);

            // 6. Store the updated counter
            await _passkeyService.UpdateCounter(res.CredentialId, res.SignCount);

            // 7. return OK to client
            return new JsonResult(res);
        }
        catch (Exception e)
        {
            return new JsonResult(new { Status = "error", ErrorMessage = FormatException(e) });
        }
    }
}

