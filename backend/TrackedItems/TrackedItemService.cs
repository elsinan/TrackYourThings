using backend.DataAccess;
using backend.DataAccess.Models;
using backend.TrackedItems;

class TrackedItemService(TytDbContext dbContext) : ITrackedItemService
{
    public async Task<TrackedItem> CreateTrackedItem(string userKey, string name)
    {
        TrackedItem item = new TrackedItem()
        await dbContext.TrackedItems.Add(name);
    }

    public Task<TrackedItem> DeleteTrackedItemById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Tuple<int, string>>> GetAllTrackedItems()
    {
        throw new NotImplementedException();
    }

    public Task<TrackedItem> GetTrackedItemById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<TrackedItem> ModifyTrackedItemName(int id, string newName)
    {
        throw new NotImplementedException();
    }
}