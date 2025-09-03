public class TrackingEntry
{

    /// <summary>
    /// Primary key of the tracking entry.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The date for which the amount is tracked.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// The tracked amount.
    /// </summary>
    public int Amount { get; set; }
}