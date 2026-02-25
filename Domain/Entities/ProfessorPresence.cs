using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities;

    public class ProfessorPresence : BaseEntity
    {
        public long ProfessorId { get; set; }
        public long UniversityId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        [IgnoreDataMember] public Professor Professor { get; set; } = null!;
        [IgnoreDataMember] public University University { get; set; } = null!;
    }

public class ProfessorPresenceConfiguration : IEntityTypeConfiguration<ProfessorPresence>
{
    public void Configure(EntityTypeBuilder<ProfessorPresence> builder)
    {
        builder.HasOne(pp => pp.Professor)
            .WithMany(p => p.Presences)
            .HasForeignKey(pp => pp.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(pp => pp.University)
            .WithMany()
            .HasForeignKey(pp => pp.UniversityId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(i => i.CreatorUser)
            .WithMany(i => i.CreatedProfessorPresences)
            .HasForeignKey(i => i.CreatorUserId);
    }
}