
using backend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess;

/// <inheritdoc />
public class TytDbContext : DbContext
{
    /// <inheritdoc />
    public TytDbContext(DbContextOptions<TytDbContext> options)
        : base(options) { }

    public DbSet<TrackedItem> TrackedItems { get; set; }
    public DbSet<TrackingEntry> TrackingEntry { get; set; }
}

