using Data.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Service.Engine.Contract;
using Shared.DTOs.Engine;

namespace Service.Engine;

public sealed class ScheduleGenerationService(
    IRepository<Assignment> assignmentRepository,
    ScheduleInputBuilder inputBuilder,
    IRepository<CourseOffering> courseOfferingRepository,
    IRepository<Professor> professorRepository,
    IRepository<TimeSlot> timeSlotRepository,
    IRepository<Course> courseRepository,
    OrToolsScheduleSolver solver)
    : IScheduleGenerationService
{
    public async Task<ScheduleResult> GenerateAsync(
        ScheduleSolveRequest request,
        CancellationToken ct = default)
    {
        if (request.DefaultRoomId <= 0)
        {
            return ScheduleResult.Failed(
                "DefaultRoomId is required because Assignment.RoomId is not nullable.");
        }

        var inputResult = await inputBuilder.BuildAsync(
            request,
            courseOfferingRepository,
            professorRepository,
            timeSlotRepository,
            courseRepository,
            ct);

        if (!inputResult.IsSuccess)
        {
            return ScheduleResult.Infeasible(inputResult.ErrorMessage);
        }

        var input = inputResult.Input!;

        var solveResult = solver.Solve(input);

        if (!solveResult.IsSuccess)
        {
            return solveResult;
        }

        // ادامه ذخیره Assignmentها
        // ...

        return solveResult;
    }
}