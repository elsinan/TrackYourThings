using backend.DataAccess;
using backend.DataAccess.Models;
using backend.TrackedItems;
using Microsoft.EntityFrameworkCore;

class TrackedItemService(TytDbContext dbContext) : ITrackedItemService
{
    public async Task<TrackedItem> CreateTrackedItem(Guid userId, string name)
    {
        var item = new TrackedItem { UserId = userId, Title = name };
        await dbContext.TrackedItems.AddAsync(item);
        await dbContext.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteTrackedItemById(Guid userId, int id)
    {
        var item = await dbContext.TrackedItems.FirstOrDefaultAsync(item => item.UserId == userId && item.Id == id);
        if (item == null) return false;
        dbContext.TrackedItems.Remove(item);
        await dbContext.SaveChangesAsync();
        return true;

    }

    public async Task<List<Tuple<int, string>>> GetAllTrackedItems(Guid userId)
    {
        return await dbContext.TrackedItems.Where(item => item.UserId == userId)
            .Select(item => new Tuple<int, string>(item.Id, item.Title))
            .ToListAsync();
    }

    public async Task<TrackedItem?> GetTrackedItemById(Guid userId, int id)
    {
        return await dbContext.TrackedItems.FirstOrDefaultAsync(item => item.UserId == userId && item.Id == id);
    }

    public async Task<bool> ModifyTrackedItemName(Guid userId, int id, string newName)
    {
        var item = await dbContext.TrackedItems.FirstOrDefaultAsync(item => item.UserId == userId && item.Id == id);
        if (item == null) return false;
        item.Title = newName;
        await dbContext.SaveChangesAsync();
        
        return true;
    }
}