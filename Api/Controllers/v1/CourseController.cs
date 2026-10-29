using Asp.Versioning;
using AutoMapper;
using Common;
using Data.Contracts;
using Domain.Entities;
using Shared.DTOs.Course;
using WebFramework.Api;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class CourseController(IRepository<Course> repository, IMapper mapper)
    : CrudController<CourseDto, CourseResDto, Course>(repository, mapper)
{
    public override async Task<ApiResult<List<SelectDto>>> GetSelectList(CancellationToken cancellationToken, string titlePropName = "Title")
    {
        return await base.GetSelectList(cancellationToken, "Name");
    }
}