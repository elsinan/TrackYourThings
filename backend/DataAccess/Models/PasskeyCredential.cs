
namespace backend.DataAccess.Models;

public class PasskeyCredential
{
    public byte[] Id { get; set; } = [];
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public byte[] PublicKey { get; set; } = [];
    public byte[] UserHandle { get; set; } = [];
    public uint SignCount { get; set; }
    public string[] Transports { get; set; } = [];
    public bool IsBackupEligible { get; set; }
    public bool IsBackedUp { get; set; }
    public string AttestationFormat { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUsedAt { get; set; }
}