using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
	public class ScheduleItemDto
	{
		public Guid Id { get; set; }
		public Guid ProfessorId { get; set; }
		public DayOfWeek Day { get; set; }
		public TimeOnly StartTime { get; set; }
		public TimeOnly EndTime { get; set; }
		public string Title { get; set; }
	}
}
