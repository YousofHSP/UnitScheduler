using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Service.Model.Contracts;
using Shared.DTOs.ProfessorDtos;

namespace Service.Model;

public class ProfessorService(
    IRepository<ProfessorSkill> professorSkillRepository,
    IMapper mapper) : IProfessorService
{

    public async Task<List<ProfessorSkillDto>> GetSkills(long professorId, CancellationToken ct)
    {
        var result = await professorSkillRepository.TableNoTracking
            .Where(i => i.ProfessorId == professorId)
            .ProjectTo<ProfessorSkillDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
        return result;
    }
    public async Task SetSkills(long professorId, long userId,List<ProfessorSkillDto> skillsDto, CancellationToken ct)
    {
        var incoming = skillsDto.Where(s => s.Id != 0).ToDictionary(s => s.Id);
        var incomingIds = incoming.Keys.ToList();

        await professorSkillRepository.Table
            .Where(i => i.ProfessorId == professorId)
            .Where(i => !incomingIds.Contains(i.Id))
            .ExecuteDeleteAsync(ct);

        var existing = await professorSkillRepository.Table
            .Where(i => i.ProfessorId == professorId)
            .ToListAsync(ct);

        foreach (var skill in existing)
        {
            if (!incoming.TryGetValue(skill.Id, out var dto))
                continue;

            skill.CourseId = dto.CourseId;
            skill.Priority = dto.Priority;
            skill.SkillLevel = dto.SkillLevel;
        }

        var newSkills = skillsDto
            .Where(s => s.Id == 0)
            .Select(dto => new ProfessorSkill
            {
                ProfessorId = professorId,
                CourseId = dto.CourseId,
                Priority = dto.Priority,
                SkillLevel = dto.SkillLevel,
                CreatorUserId = userId
            })
            .ToList();

        if (newSkills.Count > 0)
            await professorSkillRepository.AddRangeAsync(newSkills, ct);

        await professorSkillRepository.UpdateRangeAsync(existing, ct);
    }
}