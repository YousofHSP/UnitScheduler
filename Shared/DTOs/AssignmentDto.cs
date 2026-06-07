using AutoMapper;
using Domain.Entities;

namespace Shared.DTOs;

public class AssignmentDto
{
    
}

public class AssignmentResDto : BaseDto<AssignmentResDto, Assignment>
{
    public string ProfessorFullName { get; set; } = "";
    public long ProfessorId { get; set; }
    public long CourseId { get; set; }
    public string CourseName { get; set; }
    public string CourseCode { get; set; }
    public string DegreeLevelTitle { get; set; } = "";
    public string FieldTitle { get; set; } = "";
    public long TimeSlotId { get; set; }
    public DayOfWeek TimeSlotDayOfWeek{ get; set; }
    public TimeOnly TimeSlotStartTime { get; set; }
    public TimeOnly TimeSlotEndTime { get; set; }

    protected override void CustomMappings(IMappingExpression<Assignment, AssignmentResDto> mapping)
    {
        mapping.ForMember(i => i.CourseName,
            o => o.MapFrom(i => i.CourseOffering.Course.Name));
        mapping.ForMember(i => i.CourseCode,
            o => o.MapFrom(i => i.CourseOffering.Course.Code));
        mapping.ForMember(i => i.CourseCode,
            o => o.MapFrom(i => i.CourseOffering.CourseId));
        mapping.ForMember(i => i.DegreeLevelTitle,
            o => o.MapFrom(i => i.CourseOffering.Course.DegreeLevel.Title));
        mapping.ForMember(i => i.FieldTitle,
            o => o.MapFrom(i => i.CourseOffering.Course.Field.Title));
    }
}