using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities;

    public class Professor : BaseEntity
    {
        public string FullName { get; set; } = "";
        public int MaxWeeklyMinutes { get; set; }
        public long? HomeUniversityId { get; set; } // FK to University (nullable)

        // Navigation
        [IgnoreDataMember] public University HomeUniversity { get; set; } = null!;
        [IgnoreDataMember] public List<ProfessorSkill> Skills { get; set; } = [];
        [IgnoreDataMember] public List<ProfessorPresence> Presences { get; set; } = [];
        [IgnoreDataMember] public List<ProfessorAvailability> Availabilities { get; set; } = [];
        [IgnoreDataMember] public List<Assignment> Assignments { get; set; } = [];
    }


public class ProfessorConfiguration : IEntityTypeConfiguration<Professor>
{
    public void Configure(EntityTypeBuilder<Professor> builder)
    {
        builder.HasOne(p => p.HomeUniversity)
            .WithMany(u => u.Professors)
            .HasForeignKey(p => p.HomeUniversityId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(i => i.Skills)
            .WithOne(i => i.Professor)
            .HasForeignKey(i => i.ProfessorId);
        builder.HasMany(i => i.Presences)
            .WithOne(i => i.Professor)
            .HasForeignKey(i => i.ProfessorId);
        builder.HasMany(i => i.Availabilities)
            .WithOne(i => i.Professor)
            .HasForeignKey(i => i.ProfessorId);
        builder.HasMany(i => i.Assignments)
            .WithOne(i => i.Professor)
            .HasForeignKey(i => i.ProfessorId);
        builder.HasOne(i => i.CreatorUser)
            .WithMany(i => i.CreatedProfessors)
            .HasForeignKey(i => i.CreatorUserId);

    }
}