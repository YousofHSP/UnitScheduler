using Data.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Service.DataInitializer;

public class TimeSlotDataInitializer(IRepository<TimeSlot> repository) : IDataInitializer
{
    public async Task InitializerData()
    {
        var models = await repository.TableNoTracking.ToListAsync();
        var timeSlots = new Dictionary<TimeOnly, TimeOnly>
        {
            { new(8, 0), new(10, 0) },
            { new(10, 0), new(12, 0) },
            { new(12, 0), new(14, 0) },
            { new(14, 0), new(16, 0) },
            { new(16, 0), new(18, 0) }
        };
        var list = new List<TimeSlot>();
        var days = new List<DayOfWeek>()
        {
            DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Saturday, DayOfWeek.Thursday, DayOfWeek.Tuesday,
            DayOfWeek.Wednesday
        };
        foreach (var timeSlot in timeSlots)
        {
            foreach (var d in days)
            {
                list.Add(new()
                    {
                        CreatorUserId = 1,
                        StartTime = timeSlot.Key,
                        EndTime = timeSlot.Value,
                        UniversityId = 1,
                        DayOfWeek = d
                    }
                );
            }
        }

        var addingList = list.Where(i => !models.Any(m => m.DayOfWeek == i.DayOfWeek && m.StartTime == i.StartTime))
            .ToList();
        await repository.AddRangeAsync(addingList, CancellationToken.None);
    }
}