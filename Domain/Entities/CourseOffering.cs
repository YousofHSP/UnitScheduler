using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities;

    public class CourseOffering : BaseEntity
    {
        public long CourseId { get; set; }
        public long UniversityId { get; set; }
        public long TermId { get; set; }
        public int StudentCount { get; set; }
        public int GroupNumber { get; set; }  // added: parallel group identifier

        // Navigation
        [IgnoreDataMember] public Course Course { get; set; } = null!;
        [IgnoreDataMember] public University University { get; set; } = null!;
        [IgnoreDataMember] public Term Term { get; set; } = null!;
        [IgnoreDataMember] public List<CombinedCourseGroup> CombinedCourseGroups{ get; set; } = [];
        [IgnoreDataMember] public List<Assignment> Assignments { get; set; } = [];
        
    }   

public class CourseOfferingConfiguration : IEntityTypeConfiguration<CourseOffering>
{
    public void Configure(EntityTypeBuilder<CourseOffering> builder)
    {
        builder.HasOne(i => i.CreatorUser)
            .WithMany(i => i.CreatedCourseOfferings)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasOne(co => co.Course)
            .WithMany(c => c.CourseOfferings)
            .HasForeignKey(co => co.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(co => co.University)
            .WithMany(u => u.CourseOfferings)
            .HasForeignKey(co => co.UniversityId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(co => co.Term)
            .WithMany(t => t.CourseOfferings)
            .HasForeignKey(co => co.TermId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(i => i.Assignments)
            .WithOne(i => i.CourseOffering)
            .HasForeignKey(i => i.CourseOfferingId);
        builder.HasMany(i => i.CombinedCourseGroups)
            .WithMany(c => c.CourseOfferings);
    }
}