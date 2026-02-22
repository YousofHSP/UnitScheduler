using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Domain.Entities;

    public class Term : IEntity<int>
    {
        public string AcademicYear { get; set; }
        public int TermNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [IgnoreDataMember] public List<CourseOffering> CourseOfferings { get; set; }
    }

public class TermConfiguration : IEntityTypeConfiguration<Term>
{
    public void Configure(EntityTypeBuilder<Term> builder)
    {
        builder.ToTable("Terms");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.AcademicYear)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("");
            
        builder.Property(t => t.TermNumber)
            .IsRequired();
            
        builder.Property(t => t.StartDate)
            .IsRequired();
            
        builder.Property(t => t.EndDate)
            .IsRequired();
            
        builder.HasIndex(t => new { t.AcademicYear, t.TermNumber }).IsUnique();
    }
}