
using backend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess;

/// <inheritdoc />
/// <inheritdoc />
public class TytDbContext(DbContextOptions<TytDbContext> options) : DbContext(options)
{
    public DbSet<TrackedItem> TrackedItems { get; set; }
    public DbSet<TrackingEntry> TrackingEntry { get; set; }
}

