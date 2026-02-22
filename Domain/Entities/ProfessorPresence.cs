using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Domain.Entities;

    public class ProfessorPresence : IEntity<int>
    {
        public int ProfessorId { get; set; }
        public int UniversityId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        [IgnoreDataMember] public Professor Professor { get; set; }
        [IgnoreDataMember] public University University { get; set; }
    }

public class ProfessorPresenceConfiguration : IEntityTypeConfiguration<ProfessorPresence>
{
    public void Configure(EntityTypeBuilder<ProfessorPresence> builder)
    {
        builder.ToTable("ProfessorPresences");
        
        builder.HasKey(pp => pp.Id);
        
        builder.Property(pp => pp.FromDate)
            .IsRequired();
            
        builder.Property(pp => pp.ToDate)
            .IsRequired();
            
        builder.HasOne(pp => pp.Professor)
            .WithMany(p => p.Presences)
            .HasForeignKey(pp => pp.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(pp => pp.University)
            .WithMany()
            .HasForeignKey(pp => pp.UniversityId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(pp => pp.ProfessorId);
        builder.HasIndex(pp => new { pp.FromDate, pp.ToDate });
    }
}