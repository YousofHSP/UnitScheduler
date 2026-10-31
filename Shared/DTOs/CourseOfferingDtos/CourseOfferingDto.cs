using Domain.Entities;

namespace Shared.DTOs.CourseOfferingDtos;

public class CourseOfferingDto : BaseDto<CourseOfferingDto, CourseOffering>
{
    public long CourseId { get; set; }
    public long UniversityId { get; set; }
    public long TermId { get; set; }
    public int StudentCount { get; set; }
    public int GroupNumber { get; set; }
}