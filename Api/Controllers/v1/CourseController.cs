using Asp.Versioning;
using AutoMapper;
using Data.Contracts;
using Domain.Entities;
using Shared.DTOs.Course;
using WebFramework.Api;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class CourseController(IRepository<Course> repository, IMapper mapper)
    : CrudController<CourseDto, CourseResDto, Course>(repository, mapper)
{
}