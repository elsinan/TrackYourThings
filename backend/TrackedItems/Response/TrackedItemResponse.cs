namespace backend.TrackedItems.Response;


/// <summary>
/// Represents a TrackedItem.
/// </summary>
/// <param name="ItemId">The ID of the tracked item from the datastore.</param>
/// /// <param name="Title">The Title of the TrackedItem</param>
public record TrackedItemResponse(int ItemId, string Title);