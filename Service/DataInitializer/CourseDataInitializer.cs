using System.Text.Json;
using Common.Utilities;
using Data.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Service.DataInitializer;

class CourseData
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string CoMajor { get; set; }
    public string EduLevel { get; set; }
    public string Type { get; set; }
    public int Units { get; set; }
    public int Hours { get; set; }
    public int GroupCount { get; set; }
    public int Term { get; set; }
    public string PreRequisites { get; set; }
}

public class CourseDataInitializer(
    IRepository<Course> repository,
    IRepository<Field> fieldRepository,
    IRepository<DegreeLevel> degreeLevelRepository) : IDataInitializer
{
    public async Task InitializerData()
    {
        var json = File.ReadAllText(
            "E:\\Projects\\CSharp\\UniScheduling\\Service\\DataInitializer\\Data\\CourseData.json");
        var courses = JsonSerializer.Deserialize<List<CourseData>>(json);
        var fields = await fieldRepository.TableNoTracking.ToListAsync();
        var degreeLevels = await degreeLevelRepository.TableNoTracking.ToListAsync();
        var model = await repository.TableNoTracking
            .ToListAsync();
        var list = new List<Course>();
        foreach (var c in courses)
        {
            var field = fields.FirstOrDefault(i => i.Title == c.CoMajor);
            if (field == null)
                throw new Exception($"Field {c.CoMajor} not found");
            var degreeLevel = degreeLevels.FirstOrDefault(i => i.Title == c.EduLevel);
            if (degreeLevel == null)
            {
                degreeLevel = new DegreeLevel
                {
                    Title = c.EduLevel,
                    CreatorUserId = 1
                };
                degreeLevels.Add(degreeLevel);
                await degreeLevelRepository.AddAsync(degreeLevel, CancellationToken.None);
            }


            var type = Enum.Parse<CourseType>(c.Type, ignoreCase: true);
            list.Add(new()
            {
                Name = c.Name,
                Code = c.Code,
                FieldId = field.Id,
                DegreeLevelId = degreeLevel.Id,
                WeeklySessionCount = c.GroupCount,
                SessionDurationMinutes = c.Hours * 60,
                Type = type,
                Term = c.Term,
                CreatorUserId = 1
            });
        }

        var addingList = list.Where(i => model.All(m => m.Code != i.Code)).ToList();
        repository.AddRange(addingList);
        model = await repository.Table
            .Include(m => m.PreRequisites)
            .ToListAsync();
        var updatingList = new List<Course>();
        foreach (var c in model)
        {
            var course = courses.FirstOrDefault(i => i.Code == c.Code);
            if (!c.PreRequisites.Any(i => i.Code == course.PreRequisites))
            {
                var preNeed = model.FirstOrDefault(i => i.Code == course.PreRequisites);
                if (preNeed != null)
                {
                    c.PreRequisites = [preNeed];
                    updatingList.Add(c);
                }
            }
        }

        repository.UpdateRange(updatingList);
    }
}