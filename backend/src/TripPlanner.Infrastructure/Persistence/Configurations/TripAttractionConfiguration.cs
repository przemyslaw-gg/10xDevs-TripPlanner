using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Infrastructure.Persistence.Configurations;

public class TripAttractionConfiguration : IEntityTypeConfiguration<TripAttraction>
{
    public void Configure(EntityTypeBuilder<TripAttraction> builder)
    {
        builder.ToTable("trip_attractions");

        builder.HasKey(ta => ta.Id);

        builder.Property(ta => ta.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(ta => ta.TripId)
            .HasColumnName("trip_id")
            .IsRequired();

        builder.Property(ta => ta.AttractionId)
            .HasColumnName("attraction_id")
            .IsRequired();

        builder.Property(ta => ta.DayNumber)
            .HasColumnName("day_number")
            .IsRequired();

        builder.Property(ta => ta.OrderIndex)
            .HasColumnName("order_index")
            .IsRequired();

        builder.Property(ta => ta.PlannedStartTime)
            .HasColumnName("planned_start_time");

        builder.Property(ta => ta.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(ta => ta.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Relationships
        builder.HasOne(ta => ta.Trip)
            .WithMany(t => t.TripAttractions)
            .HasForeignKey(ta => ta.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ta => ta.Attraction)
            .WithMany(a => a.TripAttractions)
            .HasForeignKey(ta => ta.AttractionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Constraints
        builder.HasIndex(ta => new { ta.TripId, ta.AttractionId })
            .HasDatabaseName("uq_trip_attraction")
            .IsUnique();

        // Check constraints are handled by database migration
        builder.ToTable(t => t.HasCheckConstraint("chk_day_number_positive", "day_number > 0"));
        builder.ToTable(t => t.HasCheckConstraint("chk_order_index_non_negative", "order_index >= 0"));

        // Indexes
        builder.HasIndex(ta => new { ta.TripId, ta.DayNumber, ta.OrderIndex })
            .HasDatabaseName("idx_trip_attractions_trip_day_order");

        builder.HasIndex(ta => ta.AttractionId)
            .HasDatabaseName("idx_trip_attractions_attraction");
    }
}
