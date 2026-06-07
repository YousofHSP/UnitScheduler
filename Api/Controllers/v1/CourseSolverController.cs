using Asp.Versioning;
using Common.Exceptions;
using Common.Utilities;
using Data.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Engine.Contract;
using Shared.DTOs.Engine;
using WebFramework.Api;

namespace Api.Controllers.v1;

[ApiVersion("1.0")]
public class CourseSolverController(
    IScheduleGenerationService scheduleGenerationService, IRepository<Assignment> assignmentRepository) : BaseController
{
    [HttpPost("[action]")]
    public async Task<ApiResult<ScheduleResult>> Generate(
        [FromBody] ScheduleSolveRequest request,
        CancellationToken cancellationToken)
    {
        var result = await scheduleGenerationService.GenerateAsync(
            request,
            cancellationToken);

        if (result.Assignments.Count != 0)
        {
            var userId = User.Identity!.GetUserId<long>();
            var assignments = result.Assignments.Select(i => new Assignment
            {
                CreatorUserId = userId,
                CourseOfferingId = i.CourseOfferingId,
                ProfessorId = i.ProfessorId,
                RoomId = 1,
                TimeSlotId = i.TimeSlotId,
            });
            await assignmentRepository.AddRangeAsync(assignments, cancellationToken);
        }

        return Ok(result);
    }
}