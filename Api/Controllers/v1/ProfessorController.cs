using Asp.Versioning;
using AutoMapper;
using Data.Contracts;
using Domain.Entities;
using Shared.DTOs;
using WebFramework.Api;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class ProfessorController(IRepository<Professor> repository, IMapper mapper)
    : CrudController<ProfessorDto, ProfessorResDto, Professor>(repository, mapper)
{
    
}