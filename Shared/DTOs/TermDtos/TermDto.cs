using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Shared.Attribute;

namespace Shared.DTOs.TermDtos;

public class TermDto : BaseDto<TermDto, Term>
{
    [Display(Name = "سال تحصیلی")]
    [FormField(Order = 1)]
    [Required(ErrorMessage = "فیلد {0} احباری است")]
    public string AcademicYear { get; set; }

    [Display(Name = "شماره ترم")]
    [FormField(Order = 2, Type = FormInputType.Number)]
    [Required(ErrorMessage = "فیلد {0} احباری است")]
    public int TermNumber { get; set; }

    [Display(Name = "تاریخ شروع")]
    [FormField(Order = 3, Type = FormInputType.Date)]
    [Required(ErrorMessage = "فیلد {0} احباری است")]
    public DateTime StartDate { get; set; }

    [Display(Name = "تاریخ پایان")]
    [FormField(Order = 4, Type = FormInputType.Date)]
    [Required(ErrorMessage = "فیلد {0} احباری است")]
    public DateTime EndDate { get; set; }
}