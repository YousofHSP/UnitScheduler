using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Utilities;
using Domain.Entities;

namespace Shared.DTOs.TermDtos;

public class TermResDto : BaseDto<TermResDto, Term>
{
    [Display(Name = "سال تحصیلی")]
    public string AcademicYear { get; set; }

    [Display(Name = "شماره ترم")]
    public int TermNumber { get; set; }

    [Display(Name = "تاریخ شروع")]
    public string StartDate { get; set; }

    [Display(Name = "تاریخ پایان")]
    public string EndDate { get; set; }

    protected override void CustomMappings(IMappingExpression<Term, TermResDto> mapping)
    {
        mapping.ForMember(i => i.StartDate,
            o => o.MapFrom(i => i.StartDate.ToShamsi(false)));
        mapping.ForMember(i => i.EndDate,
            o => o.MapFrom(i => i.EndDate.ToShamsi(false)));
    }
}