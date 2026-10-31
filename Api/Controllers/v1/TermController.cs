using Asp.Versioning;
using AutoMapper;
using Common;
using Common.Utilities;
using Data.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Service.Model.Contracts;
using Shared.Attribute;
using Shared.DTOs.CourseOfferingDtos;
using Shared.DTOs.TermDtos;
using WebFramework.Api;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class TermController(IRepository<Term> repository,
    ITermService termService,
    IMapper mapper)
    : CrudController<TermDto, TermResDto, Term>(repository, mapper)
{

    [HttpGet("{termId:long}/[action]")]
    public async Task<ApiResult<List<CourseOfferingDto>>> GetCourseOfferings([FromRoute] long termId,
        CancellationToken ct)
    {
        var result = await termService.GetCourseOfferings(termId, ct);
        return Ok(result);
    }
    [HttpPost("{termId:long}/[action]")]
    public async Task<ApiResult<List<CourseOfferingDto>>> SetCourseOfferings([FromRoute] long termId,
        [FromBody] List<CourseOfferingDto> dtos,
        CancellationToken ct)
    {
        var userId = User.Identity!.GetUserId<long>();
        await termService.SetCourseOfferings(termId,dtos ,userId, ct);
        return Ok();
    }
    
    public override async Task<ApiResult<List<SelectDto>>> GetSelectList(CancellationToken cancellationToken, string titlePropName = "Title")
    {
        return await base.GetSelectList(cancellationToken, nameof(Term.TermNumber));
    }

    public override async Task<ApiResult<List<SelectOption>>> GetSelectList2(CancellationToken cancellationToken, string titlePropName = "Title")
    {
        return await base.GetSelectList2(cancellationToken, nameof(Term.TermNumber));
    }
}