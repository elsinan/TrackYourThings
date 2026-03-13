namespace backend.DataAccess.Models;

public class AppUser
{
    public Guid Id { get; set; }
    public string Username { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public byte[] UserHandle { get; set; } = Guid.NewGuid().ToByteArray();
    public List<PasskeyCredential> Credentials { get; set; } = [];



}
