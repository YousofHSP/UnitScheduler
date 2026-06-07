using Common.Utilities;
using Data.Contracts;
using Domain.Entities;
using Service.Model.Contracts;
using Shared.DTOs;

namespace Service.Model;

public class CalendarEventService : ICalendarEventService
{
    private readonly IRepository<CalendarEvent> _repository;

    public CalendarEventService(IRepository<CalendarEvent> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(CalendarEventDto dto, long userId, CancellationToken ct)
    {
        var startDateTime = dto.StartDateTime.ToMiladi().UtcDateTime;
        DateTime? endDateTime = null;
        if (!string.IsNullOrWhiteSpace(dto.EndDateTime))
        {
            endDateTime = dto.EndDateTime.ToMiladi().UtcDateTime;
            if (endDateTime < startDateTime)
                throw new ArgumentException("تاریخ پایان نباید قبل از تاریخ شروع باشد");
        }

        var model = new CalendarEvent
        {
            Title = dto.Title,
            Description = dto.Description,
            StartDateTime = startDateTime,
            EndDateTime = endDateTime,
            CreatorUserId = userId
        };

        await _repository.AddAsync(model, ct);
    }
}