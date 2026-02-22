using System.ComponentModel.DataAnnotations;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities;

public class UserGroup : BaseEntity, ISoftDelete
{
    public string Title { get; set; }
    public int? UnitCodeId { get; set; }
    public int? ParentUserGroupId { get; set; }
    public int SubSystemId { get; set; }

    public DateTimeOffset? DeleteDate { get; set; }

    #region Rels

    public UserGroup? ParentUserGroup { get; set; }
    public List<Role> Roles { get; set; } = [];
    public List<User> Users { get; set; } = [];

    #endregion
}

public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> builder)
    {

        builder.HasMany(i => i.Roles)
            .WithMany(i => i.UserGroups);

        builder.HasMany(i => i.Users)
            .WithMany(i => i.UserGroups);
        builder.HasOne(i => i.CreatorUser)
            .WithMany(i => i.CreatedUserGroups)
            .HasForeignKey(i => i.CreatorUserId);
    }
}

public enum UserGroups
{
    [Display(Name = "سیستم")]
    System = 1,
    [Display(Name = "مدیرکل")]
    Administrator,
    [Display(Name = "ادمین اصلی پروژه")]
    ProjectSuperAdmin,
    [Display(Name = "ادمین راهبری سیستم")]
    SystemAdministrator,
    [Display(Name = "هرکدام از واحدها")]
    DepartmentRepresentative,
    [Display(Name = "رئیس امور مهندسی و طراحی طرح های راه و ساختمان")]
    HeadOfEngineering,
    [Display(Name = "سرپرست طراحی پروژه")]
    ProjectDesignSupervisor,
    [Display(Name = "سرپرست معماری")]
    ArchitectureSupervisor,
    [Display(Name = "سرپرست مکانیک")]
    MechanicalSupervisor,
    [Display(Name = "سرپرست برق")]
    ElectricalSupervisor,
    [Display(Name = "سرپرست عمران")]
    CivilSupervisor,
    [Display(Name = "کارشناس معماری")]
    ArchitectureEngineer,
    [Display(Name = "کارشناس برق")]
    ElectricalEngineer,
    [Display(Name = "کارشناس سیویل")]
    CivilEngineer,
    [Display(Name = "کارشناس مکانیک")]
    MechanicalEngineer,
    [Display(Name = "سرپرست واحد نقشه کشی")]
    DraftingUnitSupervisor,
    [Display(Name = "نقشه کش معماری")]
    ArchitecturalDrafter,
    [Display(Name = "نقشه کش برق")]
    ElectricalDrafter,
    [Display(Name = "نقشه کش سیویل")]
    CivilDrafter,
    [Display(Name = "نقشه کش مکانیک")]
    MechanicalDrafter,
    [Display(Name = "سرپرست تضمین کیفیت")]
    QualityAssuranceSupervisor,
    [Display(Name = "تضمین کیفیت معماری")]
    ArchitectureQAEngineer,
    [Display(Name = "تضمین کیفیت برق")]
    ElectricalQAEngineer,
    [Display(Name = "تضمین کیفیت سیویل")]
    CivilQAEngineer,
    [Display(Name = "تضمین کیفیت مکانیک")]
    MechanicalQAEngineer,
    [Display(Name = "واحد HSE")]
    HSEUnit,
    [Display(Name = "واحد برآوردها")]
    EstimationUnit
}