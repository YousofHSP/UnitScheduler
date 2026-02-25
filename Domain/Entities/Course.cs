using System.Runtime.Serialization;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities;

    public class Course : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int WeeklySessionCount { get; set; }
        public int SessionDurationMinutes { get; set; }
        // Optional: Course type (theory, workshop, laboratory)
        public string Type { get; set; }

        // Navigation
        [IgnoreDataMember] public List<ProfessorSkill> ProfessorSkills { get; set; } = [];
        [IgnoreDataMember] public List<CourseOffering> CourseOfferings { get; set; } = null!;
        [IgnoreDataMember] public List<Course> PreRequisites { get; set; } = null!; // courses that need this as prereq
        [IgnoreDataMember] public List<Course> NeededBy { get; set; } = null!; // courses this one needs
    }

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {

        builder.HasOne(i => i.CreatorUser)
            .WithMany(i => i.CreatedCourses)
            .HasForeignKey(i => i.CreatorUserId);
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
        builder.HasMany(i => i.ProfessorSkills)
            .WithOne(i => i.Course)
            .HasForeignKey(i => i.CourseId);
        builder.HasMany(i => i.CourseOfferings)
            .WithOne(i => i.Course)
            .HasForeignKey(i => i.CourseId);
        builder.HasMany(i => i.PreRequisites)
            .WithMany(i => i.NeededBy);

    }
}