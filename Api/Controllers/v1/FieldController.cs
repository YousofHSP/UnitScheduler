using Asp.Versioning;
using AutoMapper;
using Data.Contracts;
using Domain.Entities;
using Shared.DTOs;
using WebFramework.Api;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class FieldController(IRepository<Field> repository, IMapper mapper)
    : CrudController<FieldDto, FieldResDto, Field>(repository, mapper);