using Asp.Versioning;
using AutoMapper;
using Common;
using Data.Contracts;
using Domain.Entities;
using Shared.DTOs.UniversityDtos;
using WebFramework.Api;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class UniversityController(IRepository<University> repository, IMapper mapper)
    : CrudController<UniversityDto, UniversityResDto, University>(repository, mapper)
{
    public override async Task<ApiResult<List<SelectDto>>> GetSelectList(CancellationToken cancellationToken, string titlePropName = "Title")
    {
        return await base.GetSelectList(cancellationToken, "Name");
    }
}
