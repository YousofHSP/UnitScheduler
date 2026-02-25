using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Service.Engine.Contract;
using WebFramework.Api;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class CourseSolverController(IEngineService engineService) : BaseController 
{

    [HttpGet("[action]")]
    public async Task<ApiResult> Process([FromQuery] long universityId, CancellationToken ct)
    {
        await engineService.FinalProcess(ct);
        return Ok();
    }
    
}