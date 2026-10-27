using System.ComponentModel.DataAnnotations;
using Shared.Attribute;

namespace Shared.DTOs.UniversityDtos;

public class UniversityDto : BaseDto<UniversityDto, Domain.Entities.University>
{
        
    [Display(Name = "نام")]
    [FormField(Order = 1)]
    [Required(ErrorMessage = "فیلد {0} احباری است")]
    public string Name { get; set; }
    [Display(Name = "کد")]
    [FormField(Order = 2)]
    [Required(ErrorMessage = "فیلد {0} احباری است")]
    public string Code { get; set; }
}