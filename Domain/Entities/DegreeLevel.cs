using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Domain.Entities;

    public class DegreeLevel : IEntity
    {
        public string Title { get; set; } // e.g., "کارشناسی"

        [IgnoreDataMember] public List<CourseOffering> CourseOfferings { get; set; }
    }

public class DegreeLevelConfiguration : IEntityTypeConfiguration<DegreeLevel>
{
    public void Configure(EntityTypeBuilder<DegreeLevel> builder)
    {
        builder.ToTable("DegreeLevels");
        
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("");
            
        builder.HasIndex(d => d.Title).IsUnique();
    }
}