using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Domain.Entities;

    public class Course : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WeeklySessionCount { get; set; }
        public int SessionDurationMinutes { get; set; }
        // Optional: Course type (theory, workshop, laboratory)
        public string Type { get; set; }

        // Navigation
        [IgnoreDataMember] public List<ProfessorSkill> ProfessorSkills { get; set; }
        [IgnoreDataMember] public List<CourseOffering> CourseOfferings { get; set; }
        [IgnoreDataMember] public List<CoursePreNeed> PreRequisites { get; set; }  // courses that need this as prereq
        [IgnoreDataMember] public List<CoursePreNeed> NeededBy { get; set; }       // courses this one needs
    }

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(300)
            .HasDefaultValue("");
            
        builder.Property(c => c.WeeklySessionCount)
            .IsRequired()
            .HasDefaultValue(1);
            
        builder.Property(c => c.SessionDurationMinutes)
            .IsRequired()
            .HasDefaultValue(90);
            
        builder.Property(c => c.Type)
            .HasMaxLength(50)
            .HasDefaultValue("");
    }
}