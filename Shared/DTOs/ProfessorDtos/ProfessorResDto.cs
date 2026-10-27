using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.ProfessorDtos;

public class ProfessorResDto : BaseDto<ProfessorResDto, Domain.Entities.Professor>
{
        
    [Display(Name = "نام و نام خانوادگی")]
    public string FullName { get; set; } = "";
    
    [Display(Name = "حداکثر زمان حضور در هفته (دقیقه)")]
    public int MaxWeeklyMinutes { get; set; }
    
    [Display(Name = "دانشگاه اصلی")]
    public string? HomeUniversityName { get; set; }
}