using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Infrastructure.Persistence.Configurations;

public class AttractionConfiguration : IEntityTypeConfiguration<Attraction>
{
    public void Configure(EntityTypeBuilder<Attraction> builder)
    {
        builder.ToTable("attractions");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(a => a.LocationId)
            .HasColumnName("location_id")
            .IsRequired();

        builder.Property(a => a.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasColumnName("description");

        builder.Property(a => a.Latitude)
            .HasColumnName("latitude")
            .HasPrecision(10, 7)
            .IsRequired();

        builder.Property(a => a.Longitude)
            .HasColumnName("longitude")
            .HasPrecision(10, 7)
            .IsRequired();

        builder.Property(a => a.Rating)
            .HasColumnName("rating")
            .HasPrecision(2, 1);

        builder.Property(a => a.ReviewCount)
            .HasColumnName("review_count");

        builder.Property(a => a.EstimatedDuration)
            .HasColumnName("estimated_duration");

        builder.Property(a => a.ImageUrl)
            .HasColumnName("image_url")
            .HasMaxLength(500);

        builder.Property(a => a.CreatedByUserId)
            .HasColumnName("created_by_user_id");

        builder.Property(a => a.IsVerified)
            .HasColumnName("is_verified")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Relationships
        builder.HasOne(a => a.Location)
            .WithMany(l => l.Attractions)
            .HasForeignKey(a => a.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.CreatedByUser)
            .WithMany(u => u.CreatedAttractions)
            .HasForeignKey(a => a.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(a => new { a.LocationId, a.Rating })
            .HasDatabaseName("idx_attractions_location_rating")
            .IsDescending(false, true);

        builder.HasIndex(a => a.Name)
            .HasDatabaseName("idx_attractions_name");

        builder.HasIndex(a => a.CreatedByUserId)
            .HasDatabaseName("idx_attractions_created_by")
            .HasFilter("created_by_user_id IS NOT NULL");
    }
}
