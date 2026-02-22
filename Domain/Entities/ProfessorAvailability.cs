using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Domain.Entities;

    public class ProfessorAvailability : IEntity<int>
    {
        public int ProfessorId { get; set; }
        public int UniversityId { get; set; }
        public DayOfWeek DayOfWeek { get; set; } // 0 = Saturday, 6 = Friday
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        [IgnoreDataMember] public Professor Professor { get; set; }
        [IgnoreDataMember] public University University { get; set; }
    }

public class ProfessorAvailabilityConfiguration : IEntityTypeConfiguration<ProfessorAvailability>
{
    public void Configure(EntityTypeBuilder<ProfessorAvailability> builder)
    {
        builder.ToTable("ProfessorAvailabilities");
        
        builder.HasKey(pa => pa.Id);
        
        builder.Property(pa => pa.DayOfWeek)
            .IsRequired();
            
        builder.Property(pa => pa.StartTime)
            .IsRequired();
            
        builder.Property(pa => pa.EndTime)
            .IsRequired();
            
        builder.HasOne(pa => pa.Professor)
            .WithMany(p => p.Availabilities)
            .HasForeignKey(pa => pa.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(pa => pa.University)
            .WithMany()
            .HasForeignKey(pa => pa.UniversityId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(pa => pa.ProfessorId);
    }
}