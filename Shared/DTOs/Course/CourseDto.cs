using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Shared.Attribute;

namespace Shared.DTOs.Course;

public class CourseDto : BaseDto<CourseDto, Domain.Entities.Course>
{
    [Display(Name = "نام")]
    [FormField(Order = 1)]
    [Required(ErrorMessage = "فیلد {0} احباری است")]
    public string Name { get; set; } = "";

    [Display(Name = "کد")]
    [FormField(Order = 2)]
    [Required(ErrorMessage = "فیلد {0} احباری است")]
    public string Code { get; set; } = "";

    [ListIgnore]
    [Display(Name = "رشته")]
    [FormField(Order = 3, Type = FormInputType.Select)]
    [FormOptions(Endpoint = "v1/Field/GetSelectList2", TextField = nameof(Field.Title), ValueField = nameof(Field.Id))]
    [Required(ErrorMessage = "فیلد {0} احباری است")]
    public long FieldId { get; set; }


    [ListIgnore]
    [Display(Name = "مقطع")]
    [FormField(Order = 4, Type = FormInputType.Select)]
    [FormOptions(Endpoint = "v1/DegreeLevel/GetSelectList2", TextField = nameof(DegreeLevel.Title),
        ValueField = nameof(DegreeLevel.Id))]
    [Required(ErrorMessage = "فیلد {0} احباری است")]
    public long DegreeLevelId { get; set; }


    [Display(Name = "واحد")]
    [FormField(Order = 5, Type = FormInputType.Number)]
    [Required(ErrorMessage = "فیلد {0} احباری است")]
    public int Units { get; set; }

    [Display(Name = "ترم")]
    [FormField(Order = 6, Type = FormInputType.Number)]
    [Required(ErrorMessage = "فیلد {0} احباری است")]
    public int Term { get; set; }

    [Display(Name = "تعداد درس در هفته")]
    [FormField(Order = 7, Type = FormInputType.Number)]
    [Required(ErrorMessage = "فیلد {0} احباری است")]
    public int WeeklySessionCount { get; set; }

    [Display(Name = "زمان کلاس (دقیقه)")]
    [FormField(Order = 8, Type = FormInputType.Number)]
    [Required(ErrorMessage = "فیلد {0} احباری است")]
    public int SessionDurationMinutes { get; set; }

    [Display(Name = "نوع")]
    [FormField(Order = 9)]
    [Required(ErrorMessage = "فیلد {0} احباری است")]
    public CourseType Type { get; set; }
}