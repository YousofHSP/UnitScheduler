using SchedulingService.Contracts;

namespace SchedulingService.Implementation.SearchStrategies;

public class HillClimbingStrategy : ISearchStrategy
{
    private readonly ConstraintEvaluator _evaluator;
    private readonly Random _random = new();
    private AssignmentSolution _current;
    private SchedulingContext _context;

    public AssignmentSolution BestSolution { get; private set; }

    public HillClimbingStrategy(ConstraintEvaluator evaluator)
    {
        _evaluator = evaluator;
    }

    public void Initialize(AssignmentSolution solution, SchedulingContext context)
    {
        _current = solution.Clone();
        _context = context;
        BestSolution = solution.Clone();
    }

    public bool Iterate()
    {
        var neighbor = _current.Clone();
        var movableClasses = _context.Classes.Where(c => !_current.IsFixed(c.Id)).ToList();
        if (movableClasses.Count == 0) return false;

        var cls = movableClasses[_random.Next(movableClasses.Count)];
        int newProfId = _context.Professors[_random.Next(_context.Professors.Count)].Id;
        int newRoomId = _context.Rooms[_random.Next(_context.Rooms.Count)].Id;
        int newTimeSlotId = _context.TimeSlots[_random.Next(_context.TimeSlots.Count)].Id;

        neighbor.Assignments[cls.Id] = (newProfId, newRoomId, newTimeSlotId);
        double newPenalty = _evaluator.Evaluate(neighbor, _context);

        if (newPenalty < _current.TotalPenalty)
        {
            _current = neighbor;
            _current.TotalPenalty = newPenalty;
            if (newPenalty < BestSolution.TotalPenalty)
            {
                BestSolution = neighbor.Clone();
            }
            return true;
        }
        return false;
    }
}