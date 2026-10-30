using Asp.Versioning;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Model.Contracts;
using Shared.DTOs.ProfessorDtos;
using WebFramework.Api;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class ProfessorController(
    IRepository<Professor> repository,
    IProfessorService professorService,
    IMapper mapper)
    : CrudController<ProfessorDto, ProfessorResDto, Professor>(repository, mapper)
{

    [HttpGet("{id:long}/[action]")]
    public async Task<ApiResult<List<ProfessorSkillDto>>> GetSkills([FromRoute] long id, CancellationToken ct)
    {
        var result = await professorService.GetSkills(id, ct);
        return Ok(result);
    }
    [HttpGet("{id:long}/[action]")]
    public async Task<ApiResult<List<ProfessorAvailabilityDto>>> GetAvailabilities([FromRoute] long id, CancellationToken ct)
    {
        var result = await professorService.GetAvailabilities(id, ct);
        return Ok(result);
    }

    [HttpPost("{id:long}/[action]")]
    public async Task<ApiResult> SetSkills(long id, List<ProfessorSkillDto> skillDto, CancellationToken ct)
    {
        var userId = User.Identity!.GetUserId<long>();
        await professorService.SetSkills(id,userId, skillDto, ct);
        return Ok();
    }
    
    [HttpPost("{id:long}/[action]")]
    public async Task<ApiResult> SetAvailabilities(long id, List<ProfessorAvailabilityDto> dto, CancellationToken ct)
    {
        var userId = User.Identity!.GetUserId<long>();
        await professorService.SetAvailabilities(id,userId, dto, ct);
        return Ok();
    }
    
}