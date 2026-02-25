using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities;
public class TimeSlot : BaseEntity
{
        public long UniversityId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int StartMinute { get; set; } // minutes from midnight
        public int EndMinute { get; set; }

        [IgnoreDataMember] public University University { get; set; } = null!;
        [IgnoreDataMember] public List<Assignment> Assignments { get; set; } = [];
}
    
public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.HasOne(ts => ts.University)
            .WithMany(u => u.TimeSlots)
            .HasForeignKey(ts => ts.UniversityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Assignments)
            .WithOne(i => i.TimeSlot)
            .HasForeignKey(i => i.TimeSlotId);
        builder.HasOne(i => i.CreatorUser)
            .WithMany(i => i.CreatedTimeSlots)
            .HasForeignKey(i => i.CreatorUserId);
    }
}