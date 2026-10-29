using System.ComponentModel.DataAnnotations;
using Shared.Attribute;

namespace Shared.DTOs.Engine;

public class ScheduleSolveRequest
{
    [Display(Name = "ترم")]
    [FormField(Order = 1, Type = FormInputType.Select)]
    [FormOptions(Endpoint = "v1/Term/GetSelectList2", TextField = nameof(Domain.Entities.Term.TermNumber),
        ValueField = nameof(Domain.Entities.Term.Id))]
    public long TermId { get; set; }

    [Display(Name = "دانشگاه اصلی")]
    [FormField(Order = 2, Type = FormInputType.Select)]
    [FormOptions(Endpoint = "v1/University/GetSelectList2", TextField = nameof(Domain.Entities.University.Name),
        ValueField = nameof(Domain.Entities.University.Id))]
    public long? UniversityId { get; set; } = 1;

    // [Display(Name = "")]
    // [FormField(Order = 2, Type = FormInputType.Select)]
    // [FormOptions(Endpoint = "v1/University/GetSelectList2", TextField = nameof(Domain.Entities.University.Name),
    //     ValueField = nameof(Domain.Entities.University.Id))]
    public long DefaultRoomId { get; set; } = 1;

    public bool ClearPreviousAssignments { get; set; } = true;

    public int TimeLimitSeconds { get; set; } = 60;
}