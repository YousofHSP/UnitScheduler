using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities;

    public class Assignment : BaseEntity
    {
        public long CourseOfferingId { get; set; }
        public long ProfessorId { get; set; }
        public long RoomId { get; set; }
        public long TimeSlotId { get; set; }
        public int? Score { get; set; } // optional penalty or quality measure

        // Navigation
        [IgnoreDataMember] public CourseOffering CourseOffering { get; set; } = null!;
        [IgnoreDataMember] public Professor Professor { get; set; } = null!;
        [IgnoreDataMember] public Room Room { get; set; } = null!;
        [IgnoreDataMember] public TimeSlot TimeSlot { get; set; } = null!;
    }

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {


        builder.HasOne(i => i.CreatorUser)
            .WithMany(i => i.CreatedAssignments)
            .HasForeignKey(i => i.CreatorUserId);
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
            
    }
}