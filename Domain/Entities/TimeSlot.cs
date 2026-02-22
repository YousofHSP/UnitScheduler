using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Domain.Entities;
public class TimeSlot : IEntity<int>
{
        public int UniversityId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int StartMinute { get; set; } // minutes from midnight
        public int EndMinute { get; set; }

        [IgnoreDataMember] public University University { get; set; }
        [IgnoreDataMember] public List<Assignment> Assignments { get; set; }
}
    
public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.ToTable("TimeSlots");
        
        builder.HasKey(ts => ts.Id);
        
        builder.Property(ts => ts.DayOfWeek)
            .IsRequired();
            
        builder.Property(ts => ts.StartMinute)
            .IsRequired();
            
        builder.Property(ts => ts.EndMinute)
            .IsRequired();
            
        builder.HasOne(ts => ts.University)
            .WithMany(u => u.TimeSlots)
            .HasForeignKey(ts => ts.UniversityId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(ts => new { ts.UniversityId, ts.DayOfWeek, ts.StartMinute }).IsUnique();
    }
}