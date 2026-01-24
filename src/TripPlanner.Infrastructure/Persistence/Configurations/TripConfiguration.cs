using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Infrastructure.Persistence.Configurations;

public class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.ToTable("trips");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(t => t.OwnerId)
            .HasColumnName("owner_id")
            .IsRequired();

        builder.Property(t => t.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.LocationId)
            .HasColumnName("location_id");

        builder.Property(t => t.IsPublic)
            .HasColumnName("is_public")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.DailyHours)
            .HasColumnName("daily_hours")
            .IsRequired()
            .HasDefaultValue(8);

        builder.Property(t => t.MaxExtensionHours)
            .HasColumnName("max_extension_hours")
            .IsRequired()
            .HasDefaultValue(2);

        builder.Property(t => t.StartTime)
            .HasColumnName("start_time")
            .IsRequired()
            .HasDefaultValue(new TimeOnly(9, 0));

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Relationships
        builder.HasOne(t => t.Owner)
            .WithMany(u => u.Trips)
            .HasForeignKey(t => t.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Location)
            .WithMany(l => l.Trips)
            .HasForeignKey(t => t.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(t => new { t.OwnerId, t.CreatedAt })
            .HasDatabaseName("idx_trips_owner_created")
            .IsDescending(false, true);

        builder.HasIndex(t => t.LocationId)
            .HasDatabaseName("idx_trips_location")
            .HasFilter("location_id IS NOT NULL");

        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("idx_trips_public")
            .HasFilter("is_public = true")
            .IsDescending();
    }
}
