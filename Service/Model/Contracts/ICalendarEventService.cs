using Domain.Entities;
using Shared.DTOs;

namespace Service.Model.Contracts;

public interface ICalendarEventService
{
    Task AddAsync(CalendarEventDto dto, long userId, CancellationToken ct);
}