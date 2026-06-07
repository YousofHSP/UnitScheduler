using Data.Contracts;
using Domain.Entities;

namespace Service.DataInitializer;

public class FieldDataInitializer(IRepository<Field> repository) : IDataInitializer
{
    public async Task InitializerData()
    {
        var list = new List<Field>()
        {
            new() { Title = "software", CreatorUserId = 1},
            new() { Title = "AI", CreatorUserId = 1},
            new() { Title = "IT", CreatorUserId = 1},
            new() { Title = "combine", CreatorUserId = 1}
        };
        var models = repository.TableNoTracking.ToList();
        var addingList = list.Where(i => models.All(m => m.Title != i.Title)).ToList();
        repository.AddRange(addingList);
        
    }
}