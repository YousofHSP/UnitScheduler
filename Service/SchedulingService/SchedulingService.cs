using SchedulingService.Contracts;
using SchedulingService.Implementation;

public interface ISchedulingService
{
    Task ScheduleTermAsync(int termId, CancellationToken cancellationToken = default);
}

public class SchedulingService : ISchedulingService
{
    private readonly ISchedulingRepository _repository;
    private readonly IClassWeightProvider _weightProvider;
    private readonly IEnumerable<ISolverListener> _listeners;

    public SchedulingService(
        ISchedulingRepository repository,
        IClassWeightProvider weightProvider,
        IEnumerable<ISolverListener> listeners = null)
    {
        _repository = repository;
        _weightProvider = weightProvider;
        _listeners = listeners ?? Enumerable.Empty<ISolverListener>();
    }

    public async Task ScheduleTermAsync(int termId, CancellationToken cancellationToken = default)
    {
        var config = await SolverConfiguration.LoadAsync(_repository, termId);
        var solver = new Solver(_repository, _weightProvider, config, _listeners);
        var solution = await solver.SolveAsync(termId, cancellationToken);
        var assignments = ConvertToAssignments(solution);
        await _repository.SaveAssignmentsAsync(assignments, cancellationToken);
    }

    private List<Assignment> ConvertToAssignments(AssignmentSolution solution)
    {
        var list = new List<Assignment>();
        foreach (var kv in solution.Assignments)
        {
            int classId = kv.Key;
            int offeringId = classId / 100; // reverse of encoding
            list.Add(new Assignment
            {
                CourseOfferingId = offeringId,
                ProfessorId = kv.Value.ProfessorId,
                RoomId = kv.Value.RoomId,
                TimeSlotId = kv.Value.TimeSlotId,
                Score = (int?)solution.TotalPenalty
            });
        }
        return list;
    }
}