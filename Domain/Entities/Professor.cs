using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Domain.Entities;

    public class Professor : IEntity<int>
    {
        public string FullName { get; set; }
        public int MaxWeeklyMinutes { get; set; }
        public int? HomeUniversityId { get; set; } // FK to University (nullable)

        // Navigation
        [IgnoreDataMember] public University HomeUniversity { get; set; }
        [IgnoreDataMember] public List<ProfessorSkill> Skills { get; set; }
        [IgnoreDataMember] public List<ProfessorPresence> Presences { get; set; }
        [IgnoreDataMember] public List<ProfessorAvailability> Availabilities { get; set; }
        [IgnoreDataMember] public List<Assignment> Assignments { get; set; }
    }


public class ProfessorConfiguration : IEntityTypeConfiguration<Professor>
{
    public void Configure(EntityTypeBuilder<Professor> builder)
    {
        builder.ToTable("Professors");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.FullName)
            .IsRequired()
            .HasMaxLength(200)
            .HasDefaultValue("");
            
        builder.Property(p => p.MaxWeeklyMinutes)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.HasOne(p => p.HomeUniversity)
            .WithMany(u => u.Professors)
            .HasForeignKey(p => p.HomeUniversityId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(p => p.HomeUniversityId);
    }
}