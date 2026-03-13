
using backend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess;

/// <inheritdoc />
/// <inheritdoc />
public class TytDbContext(DbContextOptions<TytDbContext> options) : DbContext(options)
{
    public DbSet<TrackedItem> TrackedItems { get; set; }
    public DbSet<TrackingEntry> TrackingEntry { get; set; }
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<PasskeyCredential> PasskeyCredentials => Set<PasskeyCredential>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<AppUser>(e => {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Username).IsUnique();
        });

        builder.Entity<PasskeyCredential>(e => {
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).HasColumnType("bytea");
            e.Property(c => c.PublicKey).HasColumnType("bytea");
            e.Property(c => c.UserHandle).HasColumnType("bytea");
            e.Property(c => c.Transports).HasColumnType("text[]");
            e.HasOne(c => c.User)
             .WithMany(u => u.Credentials)
             .HasForeignKey(c => c.UserId);
        });
    }
}

