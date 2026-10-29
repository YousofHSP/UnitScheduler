using Shared.DTOs.ProfessorDtos;

namespace Service.Model.Contracts;

public interface IProfessorService
{
    Task SetSkills(long professorId, long userId, List<ProfessorSkillDto> skillsDto, CancellationToken ct);
    Task<List<ProfessorSkillDto>> GetSkills(long professorId, CancellationToken ct);
}