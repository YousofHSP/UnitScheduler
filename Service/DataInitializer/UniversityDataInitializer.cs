using Data.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Service.DataInitializer;

public class UniversityDataInitializer(IRepository<University> repository) : IDataInitializer
{
    public async Task InitializerData()
    {
        var list = new List<University>
        {
            new() { Code = "1", Name = "دانشگاه علم و هنر یزد", CreatorUserId = 1 }
        };

        var models = await repository.TableNoTracking.ToListAsync();
        var addingList = list.Where(i => models.All(m => m.Id != i.Id)).ToList();
        await repository.AddRangeAsync(addingList, CancellationToken.None);
    }
}