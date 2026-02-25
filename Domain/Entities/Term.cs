using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities;

    public class Term : BaseEntity
    {
        public string AcademicYear { get; set; }
        public int TermNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [IgnoreDataMember] public List<CourseOffering> CourseOfferings { get; set; } = [];
    }

public class TermConfiguration : IEntityTypeConfiguration<Term>
{
    public void Configure(EntityTypeBuilder<Term> builder)
    {
        builder.HasMany(i => i.CourseOfferings)
            .WithOne(i => i.Term)
            .HasForeignKey(i => i.TermId);
        builder.HasOne(i => i.CreatorUser)
            .WithMany(i => i.CreatedTerms)
            .HasForeignKey(i => i.CreatorUserId);

    }
}