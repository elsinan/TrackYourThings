
using backend.DataAccess.Models;

namespace backend.TrackedItems;

/// <summary>
/// This is the default service for all actions related to TrackedItems.
/// </summary>
public interface ITrackedItemService
{
    /// <summary>
    /// This method returns the names and ids of all tracked items
    /// </summary>
    /// <returns></returns>
    Task<List<Tuple<int, string>>> GetAllTrackedItems();

    /// <summary>
    /// This method return a tracked item with the given id.
    /// </summary>
    /// <param name="id">the id of the tracked item which should looked up</param>
    /// <returns></returns>
    Task<TrackedItem> GetTrackedItemById(int id);

    /// <summary>
    /// This method creates a tracked item with the given name.
    /// </summary>
    /// <param name="id">the id of the tracked item which should looked up</param>
    /// <returns></returns>
    Task<TrackedItem> CreateTrackedItem(string name);

    /// <summary>
    /// This method changes the name of a tracked item.
    /// </summary>
    /// <param name="id">the id of the tracked item which should looked up</param>
    /// <returns></returns>
    Task<TrackedItem> ModifyTrackedItemName(int id, string newName);

    /// <summary>
    /// This method return a tracked item with the given id.
    /// </summary>
    /// <param name="id">the id of the tracked item which should looked up</param>
    /// <returns></returns>
    Task<TrackedItem> DeleteTrackedItemById(int id);


}