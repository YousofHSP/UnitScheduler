using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities;

    public class ProfessorAvailability : BaseEntity
    {
        public long ProfessorId { get; set; }
        public long UniversityId { get; set; }
        public DayOfWeek DayOfWeek { get; set; } // 0 = Saturday, 6 = Friday
        public int StartMinutes{ get; set; }
        public int EndMinutes{ get; set; }
        [IgnoreDataMember] public Professor Professor { get; set; } = null!;
        [IgnoreDataMember] public University University { get; set; } = null;
    }

public class ProfessorAvailabilityConfiguration : IEntityTypeConfiguration<ProfessorAvailability>
{
    public void Configure(EntityTypeBuilder<ProfessorAvailability> builder)
    {
            
        builder.HasOne(pa => pa.Professor)
            .WithMany(p => p.Availabilities)
            .HasForeignKey(pa => pa.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(pa => pa.University)
            .WithMany()
            .HasForeignKey(pa => pa.UniversityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.CreatorUser)
            .WithMany(i => i.CreatedProfessorAvailabilities)
            .HasForeignKey(i => i.CreatorUserId);
    }
}