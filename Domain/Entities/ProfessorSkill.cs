using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Domain.Entities;

    public class ProfessorSkill: IEntity
    {
        public int ProfessorId { get; set; }
        public int CourseId { get; set; }
        public int Priority { get; set; }         // lower = more preferred
        public int SkillLevel { get; set; }       // 1-5

        // Navigation
        [IgnoreDataMember] public Professor Professor { get; set; }
        [IgnoreDataMember] public Course Course { get; set; }
    }


public class ProfessorSkillConfiguration : IEntityTypeConfiguration<ProfessorSkill>
{
    public void Configure(EntityTypeBuilder<ProfessorSkill> builder)
    {
        builder.ToTable("ProfessorSkills");
        
        builder.HasKey(ps => ps.Id);
        
        builder.Property(ps => ps.Priority)
            .IsRequired()
            .HasDefaultValue(999);
            
        builder.Property(ps => ps.SkillLevel)
            .IsRequired()
            .HasDefaultValue(1);
            
        builder.HasOne(ps => ps.Professor)
            .WithMany(p => p.Skills)
            .HasForeignKey(ps => ps.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(ps => ps.Course)
            .WithMany(c => c.ProfessorSkills)
            .HasForeignKey(ps => ps.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(ps => new { ps.ProfessorId, ps.CourseId }).IsUnique();
    }
}