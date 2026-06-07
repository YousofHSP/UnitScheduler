using Data.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Service.DataInitializer;

public class CourseOfferingDataInitializer(IRepository<CourseOffering> repository, IRepository<Course> courseRepository)
    : IDataInitializer
{
    public async Task InitializerData()
    {
        var courses = await courseRepository.TableNoTracking.ToListAsync();
        var list = new List<CourseOffering>();
        int index = 1;
        foreach (var c in courses)
        {
            var model = new CourseOffering
            {
                CourseId = c.Id,
                UniversityId = 1,
                TermId = 1,
                StudentCount = 0,
                GroupNumber = index++,
                CreatorUserId = 1
            };
            list.Add(model);
        }

        var models = await repository.TableNoTracking.ToListAsync();
        var addingList = list.Where(i => models.All(m => m.CourseId != i.CourseId)).ToList();
        repository.AddRange(addingList);
    }
}