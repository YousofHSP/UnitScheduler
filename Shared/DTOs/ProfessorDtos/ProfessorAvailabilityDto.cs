using Domain.Entities;

namespace Shared.DTOs.ProfessorDtos;

public class ProfessorAvailabilityDto : BaseDto<ProfessorAvailabilityDto, ProfessorAvailability>
{
    public long ProfessorId { get; set; }
    public long UniversityId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public int StartMinutes{ get; set; }
    public int EndMinutes{ get; set; }
}