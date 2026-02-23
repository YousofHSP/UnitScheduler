using Service.SchedulingService.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.SchedulingService.Implementation
{
	public class MockSchedulingFrontendService : ISchedulingFrontendService
	{
		private readonly List<ProfessorScheduleDto> _schedules;

		public MockSchedulingFrontendService()
		{
			// Sample Data
			_schedules = new List<ProfessorScheduleDto>
			{
				new ProfessorScheduleDto
				{
					ProfessorId = Guid.NewGuid(),
					ProfessorName = "سهراب شمسی",
					Items = new List<ScheduleItemDto>
					{
						new ScheduleItemDto
						{
							Id = Guid.NewGuid(),
							ProfessorId = Guid.NewGuid(),
							Day = DayOfWeek.Monday,
							StartTime = new TimeOnly(8, 0),
							EndTime = new TimeOnly(9, 30),
							Title = "برنامه نویسی پیشرفته گروه 1"
						}
					}
				},
				new ProfessorScheduleDto
				{
					ProfessorId = Guid.NewGuid(),
					ProfessorName = "بهارنان میدیا",
					Items = new List<ScheduleItemDto>
					{
						new ScheduleItemDto
						{
							Id = Guid.NewGuid(),
							ProfessorId = Guid.NewGuid(),
							Day = DayOfWeek.Thursday,
							StartTime = new TimeOnly(10, 0),
							EndTime = new TimeOnly(11, 30),
							Title = "ریاضی گروه 1"
						}
					}
				}
                // Add more as needed
            };
		}

		public Task<List<ProfessorScheduleDto>> GetAllProfessorSchedulesAsync()
		{
			// Return a deep clone to simulate async and independence.
			var clone = _schedules.Select(s => new ProfessorScheduleDto
			{
				ProfessorId = s.ProfessorId,
				ProfessorName = s.ProfessorName,
				Items = s.Items.Select(i => new ScheduleItemDto
				{
					Id = i.Id,
					ProfessorId = i.ProfessorId,
					Day = i.Day,
					StartTime = i.StartTime,
					EndTime = i.EndTime,
					Title = i.Title
				}).ToList()
			}).ToList();
			return Task.FromResult(clone);
		}

		public Task UpdateScheduleAsync(ScheduleItemDto item)
		{
			var professor = _schedules.FirstOrDefault(p => p.ProfessorId == item.ProfessorId);
			if (professor == null) return Task.CompletedTask;

			var index = professor.Items.FindIndex(i => i.Id == item.Id);
			if (index >= 0)
				professor.Items[index] = item;
			else
				professor.Items.Add(item);

			return Task.CompletedTask;
		}
	}
}
