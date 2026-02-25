using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities;

    public class ProfessorSkill: BaseEntity 
    {
        public long ProfessorId { get; set; }
        public long CourseId { get; set; }
        public int Priority { get; set; }         // lower = more preferred
        public int SkillLevel { get; set; }       // 1-5

        // Navigation
        [IgnoreDataMember] public Professor Professor { get; set; } = null!;
        [IgnoreDataMember] public Course Course { get; set; } = null!;
    }


public class ProfessorSkillConfiguration : IEntityTypeConfiguration<ProfessorSkill>
{
    public void Configure(EntityTypeBuilder<ProfessorSkill> builder)
    {
            
        builder.HasOne(ps => ps.Professor)
            .WithMany(p => p.Skills)
            .HasForeignKey(ps => ps.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(ps => ps.Course)
            .WithMany(c => c.ProfessorSkills)
            .HasForeignKey(ps => ps.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.CreatorUser)
            .WithMany(i => i.CreatedProfessorSkills)
            .HasForeignKey(i => i.CreatorUserId);
    }
}