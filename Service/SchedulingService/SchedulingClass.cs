namespace Service.SchedulingService;

public class SchedulingClass
{
    public int Id { get; set; }
    public CourseOffering Offering { get; set; }
    public int SessionIndex { get; set; }
    public int DurationMinutes { get; set; }
    public int StudentCount => Offering.StudentCount;
    public string CohortKey => $"{Offering.FieldId}|{Offering.DegreeLevelId}|{Offering.TermId}|{Offering.GroupNumber}";
    public int CourseId => Offering.CourseId;
    public string CourseName => Offering.Course.Name;
}