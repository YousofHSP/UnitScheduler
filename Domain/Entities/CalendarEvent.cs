using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities;

public class CalendarEvent : BaseEntity, ISoftDelete
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime StartDateTime{ get; set; }
    public DateTime? EndDateTime{ get; set; }
    public DateTimeOffset? DeleteDate { get; set; }

    #region Rels

    public List<User> Users { get; set; } = [];

    #endregion
}

public class CalenderEventConfiguration : IEntityTypeConfiguration<CalendarEvent>
{
    public void Configure(EntityTypeBuilder<CalendarEvent> builder)
    {
        builder.HasOne(i => i.CreatorUser)
            .WithMany(i => i.CreatedCalendarEvents)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.Users)
            .WithMany(i => i.CalendarEvents);
    }
}