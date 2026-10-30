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
    IRepository<ProfessorAvailability> professorAvailabilityRepository,
    IMapper mapper) : IProfessorService
{
    public async Task SetAvailabilities(long professorId, long userId, List<ProfessorAvailabilityDto> dto, CancellationToken ct)
    {
        var incoming = dto.Where(s => s.Id != 0).ToDictionary(s => s.Id);
        var incomingIds = incoming.Keys.ToList();

        await professorAvailabilityRepository.Table
            .Where(i => i.ProfessorId == professorId)
            .Where(i => !incomingIds.Contains(i.Id))
            .ExecuteDeleteAsync(ct);

        var existing = await professorAvailabilityRepository.Table
            .Where(i => i.ProfessorId == professorId)
            .ToListAsync(ct);

        foreach (var item in existing)
        {
            if (!incoming.TryGetValue(item.Id, out var newDto))
                continue;

            item.UniversityId = newDto.UniversityId;
            item.StartMinutes = newDto.StartMinutes;
            item.EndMinutes = newDto.EndMinutes;
            item.DayOfWeek = newDto.DayOfWeek;
        }

        var newItems = dto 
            .Where(s => s.Id == 0)
            .Select(newDto => new ProfessorAvailability
            {
                ProfessorId = professorId,
                UniversityId= newDto.UniversityId,
                StartMinutes= newDto.StartMinutes,
                EndMinutes= newDto.EndMinutes,
                DayOfWeek = newDto.DayOfWeek,
                CreatorUserId = userId
            })
            .ToList();

        if (newItems.Count > 0)
            await professorAvailabilityRepository.AddRangeAsync(newItems, ct);

        await professorAvailabilityRepository.UpdateRangeAsync(existing, ct);        
    }

    public async Task<List<ProfessorSkillDto>> GetSkills(long professorId, CancellationToken ct)
    {
        var result = await professorSkillRepository.TableNoTracking
            .Where(i => i.ProfessorId == professorId)
            .ProjectTo<ProfessorSkillDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
        return result;
    }

    public async Task<List<ProfessorAvailabilityDto>> GetAvailabilities(long professorId, CancellationToken ct)
    {
        var result = await professorAvailabilityRepository.TableNoTracking
            .Where(i => i.ProfessorId == professorId)
            .ProjectTo<ProfessorAvailabilityDto>(mapper.ConfigurationProvider)
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