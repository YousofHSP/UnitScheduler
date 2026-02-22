using AutoMapper;
using Common.Utilities;
using Domain.Entities;

namespace Shared.DTOs;

public class CalendarEventDto : BaseDto<CalendarEventDto, CalendarEvent>
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public string StartDateTime { get; set; }
    public string? EndDateTime { get; set; }
    public int RequestStateStepId { get; set; }
    public List<int>? Users { get; set; } = [];
    public List<int>? UserGroups { get; set; } = [];
}

public class CalendarEventResDto : BaseDto<CalendarEventResDto, CalendarEvent>
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime StartDateTime { get; set; }
    public string StartJalaliDate { get; set; }

    protected override void CustomMappings(IMappingExpression<CalendarEvent, CalendarEventResDto> mapping)
    {
        mapping.ForMember(i => i.StartJalaliDate,
            o => o.MapFrom(m => m.StartDateTime.ToShamsi(true)));
    }
}