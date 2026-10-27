using Asp.Versioning;
using AutoMapper;
using Data.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.TermDtos;
using WebFramework.Api;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class TermController(IRepository<Term> repository, IMapper mapper)
    : CrudController<TermDto, TermResDto, Term>(repository, mapper)
{
    
}