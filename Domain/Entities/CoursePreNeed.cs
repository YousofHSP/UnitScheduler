/*
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Domain.Entities;

    public class CoursePreNeed : IEntity
    {
        public int CourseId { get; set; }
        public int PreNeedCourseId { get; set; }

        [IgnoreDataMember] public Course Course { get; set; }
        [IgnoreDataMember] public Course PreNeedCourse { get; set; }
    }

public class CoursePreNeedConfiguration : IEntityTypeConfiguration<CoursePreNeed>
{
    public void Configure(EntityTypeBuilder<CoursePreNeed> builder)
    {
        builder.ToTable("CoursePreNeeds");
        
        builder.HasKey(cpn => cpn.Id);
        
        builder.HasOne(cpn => cpn.Course)
            .WithMany(c => c.Prerequisites)
            .HasForeignKey(cpn => cpn.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(cpn => cpn.PreNeedCourse)
            .WithMany(c => c.NeededBy)
            .HasForeignKey(cpn => cpn.PreNeedCourseId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(cpn => new { cpn.CourseId, cpn.PreNeedCourseId }).IsUnique();
    }
}
*/
