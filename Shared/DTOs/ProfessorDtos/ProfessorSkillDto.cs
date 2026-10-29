using Domain.Entities;

namespace Shared.DTOs.ProfessorDtos;

public class ProfessorSkillDto : BaseDto<ProfessorSkillDto, ProfessorSkill>
{
    
    public long ProfessorId { get; set; }
    public long CourseId { get; set; }
    public int Priority { get; set; }
    public int SkillLevel { get; set; }

}