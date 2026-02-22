public class AssignmentSolution
{
    public Dictionary<int, (int ProfessorId, int RoomId, int TimeSlotId)> Assignments { get; set; }
        = new Dictionary<int, (int, int, int)>();
    public HashSet<int> FixedClasses { get; set; } = new HashSet<int>();
    public double TotalPenalty { get; set; }

    public AssignmentSolution Clone()
    {
        return new AssignmentSolution
        {
            Assignments = new Dictionary<int, (int, int, int)>(Assignments),
            FixedClasses = new HashSet<int>(FixedClasses),
            TotalPenalty = TotalPenalty
        };
    }

    public bool IsFixed(int classId) => FixedClasses.Contains(classId);
}