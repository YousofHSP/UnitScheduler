using System.Text.Json;
using Data.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Service.DataInitializer;

class ProfessorData
{
    public string FullName { get; set; }
    public string id { get; set; }
    public List<int> th_major { get; set; }
    public Dictionary<string, List<int>> availability { get; set; }
    public List<string> priority { get; set; }
    public int min_units { get; set; }
    public int max_units { get; set; }
}

public class ProfessorDataInitializer(
    IRepository<Professor> repository,
    IRepository<Course> courseRepository,
    IRepository<ProfessorSkill> professorSkillRepository,
    IRepository<ProfessorAvailability> professorAvailabilityRepository) : IDataInitializer
{
    public async Task InitializerData()
    {
        var json = File.ReadAllText(
            "E:\\Projects\\CSharp\\UniScheduling\\Service\\DataInitializer\\Data\\TeachersData.json");
        var professorsJson = JsonSerializer.Deserialize<List<ProfessorData>>(json);
        var courseCodeToId = await courseRepository.TableNoTracking.ToDictionaryAsync(i => i.Code, i => i.Id);
        var professors = new List<Professor>();
        var professorSkills = new List<ProfessorSkill>();
        var professorAvailabilities = new List<ProfessorAvailability>();
        foreach (var pJson in professorsJson)
        {
            // بررسی وجود استاد با همین FullName (می‌توانید با Id هم چک کنید)
            var existingProfessor = await repository.TableNoTracking
                .AnyAsync(p => p.FullName == pJson.FullName);
            if (existingProfessor) continue; // قبلاً اضافه شده

            var professor = new Professor
            {
                FullName = pJson.FullName,
                MaxWeeklyMinutes = pJson.max_units * 60, // تبدیل واحد درسی به دقیقه
                HomeUniversityId = 1,
                CreatorUserId = 1
            };

            await repository.AddAsync(professor, CancellationToken.None);

            // 1. ProfessorSkills از روی priority
            int priorityOrder = 0;
            foreach (var courseCode in pJson.priority ?? new List<string>())
            {
                if (courseCodeToId.TryGetValue(courseCode, out long courseId))
                {
                    professorSkills.Add(new ProfessorSkill
                    {
                        ProfessorId = professor.Id,
                        CourseId = courseId,
                        Priority = priorityOrder++,
                        SkillLevel = 5,
                        CreatorUserId = 1
                    });
                }
                else
                {
                    // لاگ یا نادیده گرفتن (کد درس در دیتابیس نیست)
                    Console.WriteLine($"Course code {courseCode} not found for professor {professor.FullName}");
                }
            }


            // 3. ProfessorAvailability از روی availability
            foreach (var kv in pJson.availability)
            {
                if (!Enum.TryParse<DayOfWeek>(kv.Key, out var dayOfWeek)) continue;
                var slots = kv.Value;
                if (slots == null || slots.Count == 0) continue;

                // گروه‌بندی اسلات‌های متوالی
                var sortedSlots = slots.OrderBy(s => s).ToList();
                var ranges = new List<(int start, int end)>();
                int startRange = sortedSlots[0];
                int endRange = startRange;
                for (int i = 1; i < sortedSlots.Count; i++)
                {
                    if (sortedSlots[i] == endRange + 1)
                        endRange = sortedSlots[i];
                    else
                    {
                        ranges.Add((startRange, endRange));
                        startRange = sortedSlots[i];
                        endRange = startRange;
                    }
                }

                ranges.Add((startRange, endRange));

                foreach (var (startSlot, endSlot) in ranges)
                {
                    int startMinutes = 480 + startSlot * 60; // 8:00 AM
                    int endMinutes = 480 + (endSlot + 1) * 60; // پایان آخرین اسلات
                    professorAvailabilities.Add(new ProfessorAvailability
                    {
                        ProfessorId = professor.Id,
                        UniversityId = 1,
                        DayOfWeek = dayOfWeek,
                        StartMinutes = startMinutes,
                        EndMinutes = endMinutes,
                        CreatorUserId = 1
                        
                    });
                }
            }
        }
        await professorSkillRepository.AddRangeAsync(professorSkills, CancellationToken.None);
        await professorAvailabilityRepository.AddRangeAsync(professorAvailabilities, CancellationToken.None);
    }

}