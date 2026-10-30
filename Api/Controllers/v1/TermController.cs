using Asp.Versioning;
using AutoMapper;
using Common;
using Data.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Shared.Attribute;
using Shared.DTOs.TermDtos;
using WebFramework.Api;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class TermController(IRepository<Term> repository, IMapper mapper)
    : CrudController<TermDto, TermResDto, Term>(repository, mapper)
{
    public override async Task<ApiResult<List<SelectDto>>> GetSelectList(CancellationToken cancellationToken, string titlePropName = "Title")
    {
        return await base.GetSelectList(cancellationToken, nameof(Term.TermNumber));
    }

    public override async Task<ApiResult<List<SelectOption>>> GetSelectList2(CancellationToken cancellationToken, string titlePropName = "Title")
    {
        return await base.GetSelectList2(cancellationToken, nameof(Term.TermNumber));
    }
}