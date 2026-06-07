using Asp.Versioning;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using WebFramework.Api;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class AssignmentController(IRepository<Assignment> repository, IMapper mapper) : BaseController
{
    [HttpGet("[action]")]
    public async Task<ApiResult<List<AssignmentResDto>>> Get(CancellationToken ct)
    {
        var result = await repository.TableNoTracking
            .ProjectTo<AssignmentResDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
        return Ok(result);
    }
}