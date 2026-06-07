namespace Shared.DTOs.Engine;
public sealed class ScheduleAssignmentDto
{
    public long CourseOfferingId { get; set; }
    public long ProfessorId { get; set; }
    public long TimeSlotId { get; set; }
    public long RoomId { get; set; }

    /// <summary>
    /// شماره جلسه در هفته، از صفر شروع می‌شود.
    /// </summary>
    public int SessionIndex { get; set; }

    public int? Score { get; set; }
}
