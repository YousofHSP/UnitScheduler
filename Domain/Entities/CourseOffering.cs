using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Domain.Entities;

    public class CourseOffering : IEntity<int>
    {
        public int CourseId { get; set; }
        public int UniversityId { get; set; }
        public int FieldId { get; set; }
        public int DegreeLevelId { get; set; }
        public int TermId { get; set; }
        public int StudentCount { get; set; }
        public int GroupNumber { get; set; }  // added: parallel group identifier

        // Navigation
        [IgnoreDataMember] public Course Course { get; set; }
        [IgnoreDataMember] public University University { get; set; }
        [IgnoreDataMember] public Field Field { get; set; }
        [IgnoreDataMember] public DegreeLevel DegreeLevel { get; set; }
        [IgnoreDataMember] public Term Term { get; set; }
        [IgnoreDataMember] public List<Assignment> Assignments { get; set; }
    }   

public class CourseOfferingConfiguration : IEntityTypeConfiguration<CourseOffering>
{
    public void Configure(EntityTypeBuilder<CourseOffering> builder)
    {
        builder.ToTable("CourseOfferings");
        
        builder.HasKey(co => co.Id);
        
        builder.Property(co => co.StudentCount)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(co => co.GroupNumber)
            .IsRequired()
            .HasDefaultValue(1);
            
        builder.HasOne(co => co.Course)
            .WithMany(c => c.CourseOfferings)
            .HasForeignKey(co => co.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(co => co.University)
            .WithMany(u => u.CourseOfferings)
            .HasForeignKey(co => co.UniversityId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(co => co.Field)
            .WithMany(f => f.CourseOfferings)
            .HasForeignKey(co => co.FieldId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(co => co.DegreeLevel)
            .WithMany(d => d.CourseOfferings)
            .HasForeignKey(co => co.DegreeLevelId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(co => co.Term)
            .WithMany(t => t.CourseOfferings)
            .HasForeignKey(co => co.TermId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(co => new { co.TermId, co.CourseId, co.GroupNumber });
        builder.HasIndex(co => co.FieldId);
        builder.HasIndex(co => co.DegreeLevelId);
    }
}