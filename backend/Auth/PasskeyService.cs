using backend.Auth;
using backend.DataAccess;
using backend.DataAccess.Models;
using Fido2NetLib;
using Microsoft.EntityFrameworkCore;

class PasskeyService(TytDbContext dbContext) : IPasskeyService
{
    public async Task AddCredentialToUser(Fido2User user, StoredCredential storedCredential)
    {
       storedCredential.UserId = user.Id;
        await dbContext.StoredCredentials.AddAsync(storedCredential);
    }

    public async Task<StoredCredential?> GetCredentialById(byte[] id)
    {
         var credential = await dbContext.StoredCredentials.FirstOrDefaultAsync(c => c.Descriptor.Id.SequenceEqual(id));
         return credential;
    }

    public async Task<List<StoredCredential>> GetCredentialsByUser(Fido2User user)
    {
        var credentials = await dbContext.StoredCredentials.Where(c => c.UserId.SequenceEqual(user.Id)).ToListAsync();
        return credentials;
    }

    public async Task<List<StoredCredential>> GetCredentialsByUserHandleAsync(byte[] userHandle, CancellationToken cancellationToken = default)
    {
        return await dbContext.StoredCredentials.Where(c => c.UserHandle.SequenceEqual(userHandle)).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<Fido2User> GetOrAddUser(string username, Func<Fido2User> addCallback)
    {
        var storedUser = await dbContext.StoredUsers.FirstAsync(u => u.Username == username);

        if(storedUser == null)
        {
            var newUser = addCallback();
            var newStoredUser = new StoredUser
            {
                Username = username,
                User = newUser,
            };

            await dbContext.StoredUsers.AddAsync(newStoredUser);
            return newUser!;
        } else
        {
            return storedUser.User;
        }
    }

    public async Task<Fido2User?> GetUser(string username)
    {
        var storedUser = await dbContext.StoredUsers.FirstOrDefaultAsync(u => u.Username == username);
        return storedUser?.User;
    }

    public async Task<List<Fido2User>> GetUsersByCredentialIdAsync(byte[] credentialId, CancellationToken cancellationToken  = default)
    {
        // our in-mem storage does not allow storing multiple users for a given credentialId. Yours shouldn't either.
        var cred = await dbContext.StoredCredentials.FirstOrDefaultAsync(c => c.Descriptor.Id.SequenceEqual(credentialId), cancellationToken: cancellationToken);

        if (cred is null)
            return [];

        return await dbContext.StoredUsers.Where(u => u.User.Id.SequenceEqual(cred.UserId)).Select(u => u.User).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task UpdateCounter(byte[] credentialId, uint counter)
    {
        var cred = await dbContext.StoredCredentials.FirstAsync(c => c.Descriptor.Id.SequenceEqual(credentialId));
        cred.SignCount = counter;
    }


}