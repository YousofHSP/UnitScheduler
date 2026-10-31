using Shared.DTOs.CourseOfferingDtos;

namespace Service.Model.Contracts;

public interface ITermService
{
    Task<List<CourseOfferingDto>> GetCourseOfferings(long termId, CancellationToken ct);
    Task SetCourseOfferings(long termId, List<CourseOfferingDto> dtos, long userId, CancellationToken ct);
    
}