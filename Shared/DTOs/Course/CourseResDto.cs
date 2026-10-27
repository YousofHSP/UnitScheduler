using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Shared.Attribute;

namespace Shared.DTOs.Course;

public class CourseResDto : BaseDto<CourseResDto, Domain.Entities.Course>
{
    
    [Display(Name = "نام")]
    public string Name { get; set; } = "";
    [Display(Name = "کد")]
    public string Code { get; set; } = "";
    [ListIgnore]
    public long FieldId { get; set; }
    [Display(Name = "رشته")]
    public string FieldTitle { get; set; } = "";
    [ListIgnore]
    public long DegreeLevelId { get; set; }
    [Display(Name = "مقطع")]
    public string DegreeLevelTitle { get; set; } = "";
    [Display(Name = "واحد")]
    public int Units { get; set; }
    [Display(Name = "ترم")]
    public int Term { get; set; }
    [Display(Name = "تعداد درس در هفته")]
    public int WeeklySessionCount { get; set; }
    [Display(Name = "زمان کلاس (دقیقه)")]
    public int SessionDurationMinutes { get; set; }
    [Display(Name = "نوع")]
    public CourseType Type { get; set; }
}