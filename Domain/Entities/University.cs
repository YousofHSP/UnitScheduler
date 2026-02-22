using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Domain.Entities;

public class University : IEntity<int>
{
    public string Name { get; set; }
    public string Code { get; set; }

    // Navigation properties
    public List<Professor> Professors { get; set; }
    public List<CourseOffering> CourseOfferings { get; set; }
    public List<Room> Rooms { get; set; }
    public List<TimeSlot> TimeSlots { get; set; }
}


public class UniversityConfiguration : IEntityTypeConfiguration<University>
{
    public void Configure(EntityTypeBuilder<University> builder)
    {
        builder.ToTable("Universities");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasDefaultValue("");
            
        builder.Property(u => u.Code)
            .HasMaxLength(20)
            .HasDefaultValue("");
            
        builder.HasIndex(u => u.Code).IsUnique();
    }
}