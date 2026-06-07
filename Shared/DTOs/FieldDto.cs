using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace Shared.DTOs;

public class FieldDto : BaseDto<FieldDto, Field>
{
    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public string Title { get; set; }
}

public class FieldResDto : BaseDto<FieldResDto, Field>
{
    [Display(Name = "عنوان")] public string Title { get; set; }
}