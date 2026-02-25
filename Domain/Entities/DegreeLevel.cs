using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities;

    public class DegreeLevel :BaseEntity 
    {
        public string Title { get; set; } // e.g., "کارشناسی"

        [IgnoreDataMember] public List<CourseOffering> CourseOfferings { get; set; } = []; 
    }

public class DegreeLevelConfiguration : IEntityTypeConfiguration<DegreeLevel>
{
    public void Configure(EntityTypeBuilder<DegreeLevel> builder)
    {
        builder.HasOne(i => i.CreatorUser)
            .WithMany(i => i.CreatedDegreeLevels)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.CourseOfferings)
            .WithOne(i => i.DegreeLevel)
            .HasForeignKey(i => i.DegreeLevelId);

    }
}