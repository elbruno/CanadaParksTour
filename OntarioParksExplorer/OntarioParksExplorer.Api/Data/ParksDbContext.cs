using Microsoft.EntityFrameworkCore;
using OntarioParksExplorer.Api.Data.Entities;

namespace OntarioParksExplorer.Api.Data;

public class ParksDbContext : DbContext
{
    public ParksDbContext(DbContextOptions<ParksDbContext> options)
        : base(options)
    {
    }

    public DbSet<Park> Parks => Set<Park>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<ParkActivity> ParkActivities => Set<ParkActivity>();
    public DbSet<ParkImage> ParkImages => Set<ParkImage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Park entity
        modelBuilder.Entity<Park>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).IsRequired();
            entity.Property(p => p.Location).IsRequired().HasMaxLength(500);
            entity.Property(p => p.Region).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Website).HasMaxLength(500);
            
            // Indexes
            entity.HasIndex(p => p.Name);
            entity.HasIndex(p => p.Region);
            entity.HasIndex(p => p.IsFeatured);

            // Auto-update timestamps
            entity.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(p => p.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Configure Activity entity
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            
            // Index
            entity.HasIndex(a => a.Name).IsUnique();
        });

        // Configure ParkActivity many-to-many relationship
        modelBuilder.Entity<ParkActivity>(entity =>
        {
            entity.HasKey(pa => new { pa.ParkId, pa.ActivityId });

            entity.HasOne(pa => pa.Park)
                .WithMany(p => p.ParkActivities)
                .HasForeignKey(pa => pa.ParkId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pa => pa.Activity)
                .WithMany(a => a.ParkActivities)
                .HasForeignKey(pa => pa.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ParkImage entity
        modelBuilder.Entity<ParkImage>(entity =>
        {
            entity.HasKey(pi => pi.Id);
            entity.Property(pi => pi.Url).IsRequired().HasMaxLength(1000);
            entity.Property(pi => pi.AltText).HasMaxLength(500);

            entity.HasOne(pi => pi.Park)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ParkId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(pi => pi.ParkId);
        });
    }
}
