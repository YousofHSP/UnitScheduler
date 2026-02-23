using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
	public class ProfessorScheduleDto
	{
		public Guid ProfessorId { get; set; }
		public string ProfessorName { get; set; }
		public List<ScheduleItemDto> Items { get; set; } = new();
	}
}
