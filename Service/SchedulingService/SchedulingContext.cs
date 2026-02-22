public class SchedulingContext
{
    public List<SchedulingClass> Classes { get; set; }
    public List<Professor> Professors { get; set; }
    public List<Room> Rooms { get; set; }
    public List<TimeSlot> TimeSlots { get; set; }

    public Dictionary<int, Professor> ProfessorById { get; set; }
    public Dictionary<int, Room> RoomById { get; set; }
    public Dictionary<int, TimeSlot> TimeSlotById { get; set; }
    public Dictionary<int, List<ProfessorSkill>> SkillsByCourseId { get; set; }
    public Dictionary<int, Dictionary<DayOfWeek, List<ProfessorAvailability>>> AvailabilityByProfessorId { get; set; }
    public Dictionary<int, (int ProfessorId, int RoomId, int TimeSlotId)> CommittedAssignments { get; set; } = new();

    // Optional: student enrollments
    public Dictionary<int, List<int>> StudentEnrollments { get; set; } = new();

    public SchedulingContext(
        List<CourseOffering> offerings,
        List<Professor> professors,
        List<ProfessorSkill> skills,
        List<ProfessorAvailability> availabilities,
        List<Room> rooms,
        List<TimeSlot> timeSlots,
        List<Assignment> committed = null)
    {
        // Build Classes
        Classes = new List<SchedulingClass>();
        foreach (var off in offerings)
        {
            for (int i = 0; i < off.Course.WeeklySessionCount; i++)
            {
                Classes.Add(new SchedulingClass
                {
                    Id = off.Id * 100 + i,
                    Offering = off,
                    SessionIndex = i,
                    DurationMinutes = off.Course.SessionDurationMinutes
                });
            }
        }

        Professors = professors;
        Rooms = rooms;
        TimeSlots = timeSlots;

        ProfessorById = professors.ToDictionary(p => p.Id);
        RoomById = rooms.ToDictionary(r => r.Id);
        TimeSlotById = timeSlots.ToDictionary(ts => ts.Id);

        SkillsByCourseId = skills
            .GroupBy(s => s.CourseId)
            .ToDictionary(g => g.Key, g => g.ToList());

        AvailabilityByProfessorId = availabilities
            .GroupBy(a => a.ProfessorId)
            .ToDictionary(
                g => g.Key,
                g => g.GroupBy(a => a.DayOfWeek)
                      .ToDictionary(dg => dg.Key, dg => dg.ToList())
            );

        if (committed != null)
        {
            foreach (var a in committed)
            {
                var cls = Classes.FirstOrDefault(c => c.Offering.Id == a.CourseOfferingId);
                if (cls != null)
                {
                    CommittedAssignments[cls.Id] = (a.ProfessorId, a.RoomId, a.TimeSlotId);
                }
            }
        }
    }
}