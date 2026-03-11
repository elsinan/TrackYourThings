using Fido2NetLib;

namespace backend.DataAccess.Models;

public class StoredUser
{
    /// <summary>
    /// The Credential ID of the user.
    /// </summary>
    public int Id { get; set; }


    public required string Username { get; set; }

    public required Fido2User User { get; set; }


}
