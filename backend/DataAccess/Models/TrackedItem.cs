
namespace backend.DataAccess.Models;

public class TrackedItem
{
    // [SetsRequiredMembers]
    // public TrackedItem(string userKey, string? title)
    // {
    //     this.UserKey = userKey;
    //     this.Title = title;
    // }

    /// <summary>
    /// Primary key of the tracked item.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Title of the tracked item.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Creator of the tracked item.
    /// </summary>
    public required string UserKey { get; set; }

    /// <summary>
    /// Entries of the tracked item.
    /// </summary>
    public List<TrackingEntry> Entries { get; set; } = [];
}