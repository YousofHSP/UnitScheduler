using SchedulingService.Contracts;
using SchedulingService.Implementation.Constraints;
using SchedulingService.Implementation.SearchStrategies;
using SchedulingService.Implementation.WeightProviders;

namespace SchedulingService.Implementation;

public class Solver : ISolver
{
    private readonly ISchedulingRepository _repository;
    private readonly ConstraintEvaluator _evaluator;
    private readonly IClassWeightProvider _weightProvider;
    private readonly SolverConfiguration _config;
    private readonly List<ISolverListener> _listeners;

    public Solver(
        ISchedulingRepository repository,
        IClassWeightProvider weightProvider,
        SolverConfiguration config,
        IEnumerable<ISolverListener> listeners = null)
    {
        _repository = repository;
        _weightProvider = weightProvider;
        _config = config;
        _listeners = listeners?.ToList() ?? new List<ISolverListener>();
        _evaluator = CreateEvaluator(config);
    }

    private ConstraintEvaluator CreateEvaluator(SolverConfiguration config)
    {
        var constraints = new List<(IConstraint, double)>
        {
            (new ProfessorAvailabilityConstraint(), double.MaxValue),
            (new ProfessorSkillConstraint(), double.MaxValue),
            (new NoOverlapConstraint(), double.MaxValue),
            (new ProfessorPriorityConstraint(), config.ConstraintWeights.GetValueOrDefault("ProfessorPriority", 1.0)),
            (new RoomCapacityConstraint(), config.ConstraintWeights.GetValueOrDefault("RoomCapacity", 1.0)),
        };
        // Add student conflict if needed
        return new ConstraintEvaluator(constraints);
    }

    public async Task<AssignmentSolution> SolveAsync(int termId, CancellationToken cancellationToken = default)
    {
        // 1. Load data
        var offerings = await _repository.GetCourseOfferingsAsync(termId, cancellationToken);
        var professors = await _repository.GetProfessorsAsync(cancellationToken);
        var skills = await _repository.GetProfessorSkillsAsync(cancellationToken);
        var presences = await _repository.GetProfessorPresencesAsync(termId, cancellationToken);
        var availabilities = await _repository.GetProfessorAvailabilitiesAsync(cancellationToken);
        var rooms = await _repository.GetRoomsAsync(cancellationToken);
        var timeSlots = await _repository.GetTimeSlotsAsync(cancellationToken);
        var committedAssignments = await _repository.GetAssignmentsForTermAsync(termId, cancellationToken);

        // Filter professors present in this term
        var presentProfessorIds = presences.Select(p => p.ProfessorId).ToHashSet();
        professors = professors.Where(p => presentProfessorIds.Contains(p.Id)).ToList();

        // 2. Build context
        var context = new SchedulingContext(offerings, professors, skills, availabilities, rooms, timeSlots, committedAssignments);

        // 3. Create initial solution (greedy with weight ordering)
        var initial = GreedyConstruct(context);

        // 4. Mark fixed classes from committed assignments
        foreach (var kv in context.CommittedAssignments)
        {
            initial.Assignments[kv.Key] = kv.Value;
            initial.FixedClasses.Add(kv.Key);
        }
        initial.TotalPenalty = _evaluator.Evaluate(initial, context);

        // 5. Choose search strategy
        ISearchStrategy strategy = _config.SearchStrategy.ToLower() switch
        {
            "simulatedannealing" => new SimulatedAnnealingStrategy(_evaluator, _config.InitialTemperature, _config.CoolingRate),
            _ => new HillClimbingStrategy(_evaluator)
        };
        strategy.Initialize(initial, context);

        // 6. Run iterations
        for (int iter = 0; iter < _config.MaxIterations; iter++)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            bool improved = strategy.Iterate();
            if (iter % 1000 == 0)
            {
                foreach (var listener in _listeners)
                    listener.OnProgress(iter, strategy.BestSolution.TotalPenalty, strategy.BestSolution.TotalPenalty);
            }
        }

        var final = strategy.BestSolution;
        final.TotalPenalty = _evaluator.Evaluate(final, context);
        return final;
    }

    private AssignmentSolution GreedyConstruct(SchedulingContext context)
    {
        var solution = new AssignmentSolution();
        var random = new Random();
        var orderedClasses = context.Classes.OrderByDescending(c => _weightProvider.GetWeight(c)).ToList();

        foreach (var cls in orderedClasses)
        {
            bool assigned = false;
            var professors = context.Professors.OrderBy(p => random.Next()).ToList();
            var rooms = context.Rooms.OrderBy(r => random.Next()).ToList();
            var timeSlots = context.TimeSlots.OrderBy(ts => random.Next()).ToList();

            foreach (var prof in professors)
            foreach (var room in rooms)
            foreach (var ts in timeSlots)
            {
                solution.Assignments[cls.Id] = (prof.Id, room.Id, ts.Id);
                var penalty = _evaluator.Evaluate(solution, context);
                if (penalty < double.MaxValue)
                {
                    assigned = true;
                    break;
                }
                solution.Assignments.Remove(cls.Id);
            }
            if (!assigned)
                throw new InvalidOperationException($"Cannot find feasible assignment for class {cls.Id}");
        }
        return solution;
    }
}