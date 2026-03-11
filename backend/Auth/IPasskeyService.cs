using backend.DataAccess.Models;
using Fido2NetLib;

namespace backend.Auth;

public interface IPasskeyService
{
    public Task<Fido2User> GetOrAddUser(string username, Func<Fido2User> addCallback);
    public Task<Fido2User?> GetUser(string username);
    public Task<List<StoredCredential>> GetCredentialsByUser(Fido2User user);
    public Task<StoredCredential?> GetCredentialById(byte[] id);
    public Task<List<StoredCredential>> GetCredentialsByUserHandleAsync(byte[] userHandle, CancellationToken cancellationToken = default);
    public Task UpdateCounter(byte[] credentialId, uint counter);
    public Task AddCredentialToUser(Fido2User user, StoredCredential storedCredential);
    public Task<List<Fido2User>> GetUsersByCredentialIdAsync(byte[] credentialId, CancellationToken cancellationToken);
}