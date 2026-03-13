
namespace backend.DataAccess.Models;

public class TrackedItem
{

    /// <summary>
    /// Primary key of the tracked item.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Title of the tracked item.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Creator of the tracked item.
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    /// Entries of the tracked item.
    /// </summary>
    public List<TrackingEntry> Entries { get; set; } = [];
}