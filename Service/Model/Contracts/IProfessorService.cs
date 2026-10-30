using Shared.DTOs.ProfessorDtos;

namespace Service.Model.Contracts;

public interface IProfessorService
{
    Task SetSkills(long professorId, long userId, List<ProfessorSkillDto> skillsDto, CancellationToken ct);
    Task SetAvailabilities(long professorId, long userId, List<ProfessorAvailabilityDto> dto, CancellationToken ct);
    Task<List<ProfessorSkillDto>> GetSkills(long professorId, CancellationToken ct);
    Task<List<ProfessorAvailabilityDto>> GetAvailabilities(long professorId, CancellationToken ct);
}