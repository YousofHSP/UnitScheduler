using System.ComponentModel.DataAnnotations;
using Shared.Attribute;

namespace Shared.DTOs.ProfessorDtos;

public class ProfessorDto : BaseDto<ProfessorDto, Domain.Entities.Professor>
{
    
    [Display(Name = "نام و نام خانوادگی")]
    [FormField(Order = 1)]
    [Required(ErrorMessage = "فیلد {0} احباری است")]
    public string FullName { get; set; } = "";
    
    [Display(Name = "حداکثر زمان حضور در هفته (دقیقه)")]
    [FormField(Order = 2, Type = FormInputType.Number)]
    [Required(ErrorMessage = "فیلد {0} احباری است")]
    public int MaxWeeklyMinutes { get; set; }
    
    [Display(Name = "دانشگاه اصلی")]
    [FormField(Order = 3, Type = FormInputType.Select)]
    [FormOptions(Endpoint = "v1/University/GetSelectList2", TextField = nameof(Domain.Entities.University.Name),
        ValueField = nameof(Domain.Entities.University.Id))]
    public long? HomeUniversityId { get; set; }
}