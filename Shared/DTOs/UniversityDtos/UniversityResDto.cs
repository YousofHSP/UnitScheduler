using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.UniversityDtos;

public class UniversityResDto : BaseDto<UniversityResDto, Domain.Entities.University>
{
    
    [Display(Name = "نام")]
    public string Name { get; set; }
    [Display(Name = "کد")]
    public string Code { get; set; }
}