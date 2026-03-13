using backend.DataAccess.Models;
using Fido2NetLib;

namespace backend.Auth;

public interface IPasskeyService
{
    Task<CredentialCreateOptions> BeginRegistrationAsync(string username);
    Task<string> CompleteRegistrationAsync(string username, AuthenticatorAttestationRawResponse attestation);
    Task<AssertionOptions> BeginLoginAsync(string username);
    Task<string> CompleteLoginAsync(AuthenticatorAssertionRawResponse assertion);
    public Task<AppUser?> GetUserBySessionAsync(string sessionId);
}