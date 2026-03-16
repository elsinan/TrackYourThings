
using backend.DataAccess.Models;

namespace backend.TrackingEntries;

/// <summary>
/// This is the default service for all actions related to TrackedItems.
/// </summary>
public interface ITrackingEntryService
{

    /// <summary>
    /// This method creates a tracked item with the given name.
    /// </summary>
    /// <param name="id">the id of the tracked item which should looked up</param>
    /// <param name="amount">the amount to track</param>
    /// <param name="date">the date of the tracking entry</param>
    /// <returns></returns>
    Task<TrackedItem> AddOrModifyTrackingEntry(int id, int amount, DateTime date);


    /// <summary>
    /// This method return a tracked item with the given id.
    /// </summary>
    /// <param name="id">the id of the tracked item which should looked up</param>
    /// <param name="date">the date of the tracking entry</param>
    /// <returns></returns>
    Task<TrackedItem> DeleteTrackingEntryById(int id, DateTime date);
}