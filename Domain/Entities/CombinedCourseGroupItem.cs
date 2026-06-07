using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities;

public class CombinedCourseGroup : BaseEntity
{
    public string Title { get; set; }

    #region Rels

    public List<CourseOffering> CourseOfferings { get; set; } = [];

    #endregion
}
public class CombinedCourseGroupConfiguration : IEntityTypeConfiguration<CombinedCourseGroup>
{
    public void Configure(EntityTypeBuilder<CombinedCourseGroup> builder)
    {
        builder.HasOne(i => i.CreatorUser)
            .WithMany(i => i.CreatedCombinedCourseGroups)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.CourseOfferings)
            .WithMany(i => i.CombinedCourseGroups);
    }
}