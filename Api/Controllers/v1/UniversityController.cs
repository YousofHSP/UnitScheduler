using Asp.Versioning;
using AutoMapper;
using Data.Contracts;
using Domain.Entities;
using Shared.DTOs.UniversityDtos;
using WebFramework.Api;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class UniversityController(IRepository<University> repository, IMapper mapper)
    : CrudController<UniversityDto, UniversityResDto, University>(repository, mapper)
{
    
}
