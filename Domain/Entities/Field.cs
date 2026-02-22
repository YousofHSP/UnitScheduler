using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Domain.Entities;

    public class Field : IEntity
    {
        public string Title { get; set; } // e.g., "مهندسی نرم‌افزار"

        [IgnoreDataMember] public List<CourseOffering> CourseOfferings { get; set; }
    }


public class FieldConfiguration : IEntityTypeConfiguration<Field>
{
    public void Configure(EntityTypeBuilder<Field> builder)
    {
        builder.ToTable("Fields");
        
        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.Title)
            .IsRequired()
            .HasMaxLength(200)
            .HasDefaultValue("");
            
        builder.HasIndex(f => f.Title).IsUnique();
    }
}