using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Service.Model.Contracts;
using Shared.DTOs.CourseOfferingDtos;

namespace Service.Model;

public class TermService(
    IRepository<CourseOffering> courseOfferingRepository,
    IMapper mapper) : ITermService
{
    public async Task<List<CourseOfferingDto>> GetCourseOfferings(long termId, CancellationToken ct)
    {
        return await courseOfferingRepository.TableNoTracking
            .Where(i => i.TermId == termId)
            .ProjectTo<CourseOfferingDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task SetCourseOfferings(long termId, List<CourseOfferingDto> dtos, long userId, CancellationToken ct)
    {
        var incoming = dtos.Where(s => s.Id != 0).ToDictionary(s => s.Id);
        var incomingIds = incoming.Keys.ToList();

        await courseOfferingRepository.Table
            .Where(i => i.TermId == termId)
            .Where(i => !incomingIds.Contains(i.Id))
            .ExecuteDeleteAsync(ct);

        var existing = await courseOfferingRepository.Table
            .Where(i => i.TermId == termId)
            .ToListAsync(ct);

        existing = existing.Select(i =>
        {
            if (!incoming.TryGetValue(i.Id, out var newDto))
                return i;
            return newDto.ToEntity(i, mapper);
        }).ToList();

        var newItems = dtos 
            .Where(s => s.Id == 0)
            .Select(newDto =>
            {
                
                var i = newDto.ToEntity(mapper);
                i.CreatorUserId = userId;
                return i;
            })
            .ToList();

        if (newItems.Count > 0)
            await courseOfferingRepository.AddRangeAsync(newItems, ct);

        await courseOfferingRepository.UpdateRangeAsync(existing, ct);        
    }
}