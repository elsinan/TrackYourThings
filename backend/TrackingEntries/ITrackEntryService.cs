
using backend.DataAccess.Models;

namespace backend.TrackingEntries;

/// <summary>
/// This is the default service for all actions related to TrackedItems.
/// </summary>
public interface ITrackingEntryService
{
    /// <summary>
    /// This method returns the all tracking entries of a given track item.
    /// </summary>
    /// <returns></returns>
    Task<List<TrackingEntry>> GetAllTrackingEntriesById(int id);

    /// <summary>
    /// This method creates a tracked item with the given name.
    /// </summary>
    /// <param name="id">the id of the tracked item which should looked up</param>
    /// <returns></returns>
    Task<TrackedItem> CreateTrackedItem(int id, TrackingEntry newItem);

    /// <summary>
    /// This method changes the name of a tracked item.
    /// </summary>
    /// <param name="id">the id of the tracked item which should looked up</param>
    /// <returns></returns>
    Task<TrackedItem> ModifyTrackingItem(int id, string newName);

    /// <summary>
    /// This method return a tracked item with the given id.
    /// </summary>
    /// <param name="id">the id of the tracked item which should looked up</param>
    /// <returns></returns>
    Task<TrackedItem> DeleteTrackingEntryById(int id);


}