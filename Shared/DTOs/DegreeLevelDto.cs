using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace Shared.DTOs;

public class DegreeLevelDto : BaseDto<DegreeLevelDto, DegreeLevel>
{
    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public string Title { get; set; } = "";
}
public class DegreeLevelResDto : BaseDto<DegreeLevelResDto, DegreeLevel>
{
    [Display(Name = "عنوان")] public string Title { get; set; } = "";
}
