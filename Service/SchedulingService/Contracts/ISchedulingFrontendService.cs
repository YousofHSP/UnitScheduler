using System;
using System.Collections.Generic;
using System.Text;

namespace Service.SchedulingService.Contracts
{
	public interface ISchedulingFrontendService
	{
		Task<List<ProfessorScheduleDto>> GetAllProfessorSchedulesAsync();
		Task UpdateScheduleAsync(ScheduleItemDto item);
	}
}
