using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Domain.Entities;

    public class Assignment : IEntity<int>
    {
        public int CourseOfferingId { get; set; }
        public int ProfessorId { get; set; }
        public int RoomId { get; set; }
        public int TimeSlotId { get; set; }
        public int? Score { get; set; } // optional penalty or quality measure

        // Navigation
        [IgnoreDataMember] public CourseOffering CourseOffering { get; set; }
        [IgnoreDataMember] public Professor Professor { get; set; }
        [IgnoreDataMember] public Room Room { get; set; }
        [IgnoreDataMember] public TimeSlot TimeSlot { get; set; }
    }

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.ToTable("Assignments");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Score)
            .HasDefaultValue(null);
            
        builder.HasOne(a => a.CourseOffering)
            .WithMany(co => co.Assignments)
            .HasForeignKey(a => a.CourseOfferingId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(a => a.Professor)
            .WithMany(p => p.Assignments)
            .HasForeignKey(a => a.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(a => a.Room)
            .WithMany(r => r.Assignments)
            .HasForeignKey(a => a.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(a => a.TimeSlot)
            .WithMany(ts => ts.Assignments)
            .HasForeignKey(a => a.TimeSlotId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(a => new { a.CourseOfferingId });
        builder.HasIndex(a => new { a.ProfessorId, a.TimeSlotId }).IsUnique();
        builder.HasIndex(a => new { a.RoomId, a.TimeSlotId }).IsUnique();
    }
}