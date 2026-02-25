using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities;

    public class Field : BaseEntity 
    {
        public string Title { get; set; } // e.g., "مهندسی نرم‌افزار"

        [IgnoreDataMember] public List<CourseOffering> CourseOfferings { get; set; } = [];
    }


public class FieldConfiguration : IEntityTypeConfiguration<Field>
{
    public void Configure(EntityTypeBuilder<Field> builder)
    {

        builder.HasMany(i => i.CourseOfferings)
            .WithOne(i => i.Field)
            .HasForeignKey(i => i.FieldId);
        builder.HasOne(i => i.CreatorUser)
            .WithMany(i => i.CreatedFields)
            .HasForeignKey(i => i.CreatorUserId);
    }
}