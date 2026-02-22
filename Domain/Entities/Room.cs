using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Domain.Entities;

    public class Room : IEntity<int>
    {
        public int UniversityId { get; set; }
        public int Capacity { get; set; }

        [IgnoreDataMember] public University University { get; set; }
        [IgnoreDataMember] public List<Assignment> Assignments { get; set; }
    }

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Capacity)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.HasOne(r => r.University)
            .WithMany(u => u.Rooms)
            .HasForeignKey(r => r.UniversityId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(r => r.UniversityId);
    }
}