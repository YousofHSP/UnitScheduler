using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities;

    public class Room : BaseEntity
    {
        public long UniversityId { get; set; }
        public int Capacity { get; set; }

        [IgnoreDataMember] public University University { get; set; } = null!;
        [IgnoreDataMember] public List<Assignment> Assignments { get; set; } = [];
    }

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasOne(r => r.University)
            .WithMany(u => u.Rooms)
            .HasForeignKey(r => r.UniversityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Assignments)
            .WithOne(i => i.Room)
            .HasForeignKey(i => i.RoomId);
        builder.HasOne(i => i.CreatorUser)
            .WithMany(i => i.CreatedRooms)
            .HasForeignKey(i => i.CreatorUserId);
    }
}