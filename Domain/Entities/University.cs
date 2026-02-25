using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities;

public class University : BaseEntity
{
    public string Name { get; set; }
    public string Code { get; set; }

    // Navigation properties
    public List<Professor> Professors { get; set; } = [];
    public List<CourseOffering> CourseOfferings { get; set; } = [];
    public List<Room> Rooms { get; set; } = [];
    public List<TimeSlot> TimeSlots { get; set; } = [];
}


public class UniversityConfiguration : IEntityTypeConfiguration<University>
{
    public void Configure(EntityTypeBuilder<University> builder)
    {
        builder.HasOne(i => i.CreatorUser)
            .WithMany(i => i.CreatedUniversities)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.Professors)
            .WithOne(i => i.HomeUniversity)
            .HasForeignKey(i => i.HomeUniversityId);
        builder.HasMany(i => i.CourseOfferings)
            .WithOne(i => i.University)
            .HasForeignKey(i => i.UniversityId);
        builder.HasMany(i => i.Rooms)
            .WithOne(i => i.University)
            .HasForeignKey(i => i.UniversityId);
        builder.HasMany(i => i.TimeSlots)
            .WithOne(i => i.University)
            .HasForeignKey(i => i.UniversityId);
        
    }
}