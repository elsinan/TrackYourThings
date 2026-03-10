public class UserKeys
{
    /// <summary>
    /// Primary key of the User key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The date for which the amount is tracked.
    /// </summary>
    public required string Key{ get; set; }
}