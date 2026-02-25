using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Domain.Entities.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities;


[Display(Name = "کاربران")]
public class User : IdentityUser<long>, IBaseEntity<long> 
{
    public DateTimeOffset CreateDate { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset? DeleteDate { get; set; } = null;
    public bool Enable { get; set; }

    [IgnoreDataMember] public List<UserGroup> UserGroups { get; set; } = [];
    [IgnoreDataMember] public UserInfo? Info { get; set; }

    [IgnoreDataMember] public List<ApiToken> ApiTokens { get; set; } = [];
    [IgnoreDataMember] public List<SmsLog> ReceivedSms { get; set; } = [];
    [IgnoreDataMember] public List<Notification> Notifications { get; set; } = [];
    [IgnoreDataMember] public List<CalendarEvent> CalendarEvents { get; set; } = [];
    [IgnoreDataMember] public List<CalendarEvent> CreatedCalendarEvents { get; set; } = [];
    [IgnoreDataMember] public List<UserGroup> CreatedUserGroups { get; set; } = [];
    [IgnoreDataMember] public List<Assignment> CreatedAssignments { get; set; } = [];
    [IgnoreDataMember] public List<CourseOffering> CreatedCourseOfferings { get; set; } = [];
    [IgnoreDataMember] public List<Course> CreatedCourses{ get; set; } = [];
    [IgnoreDataMember] public List<DegreeLevel> CreatedDegreeLevels { get; set; } = [];
    [IgnoreDataMember] public List<Field> CreatedFields { get; set; } = [];
    [IgnoreDataMember] public List<Professor> CreatedProfessors { get; set; } = [];
    [IgnoreDataMember] public List<ProfessorAvailability> CreatedProfessorAvailabilities { get; set; } = [];
    [IgnoreDataMember] public List<ProfessorPresence> CreatedProfessorPresences { get; set; } = [];
    [IgnoreDataMember] public List<ProfessorSkill> CreatedProfessorSkills { get; set; } = [];
    [IgnoreDataMember] public List<Room> CreatedRooms { get; set; } = [];
    [IgnoreDataMember] public List<Term> CreatedTerms { get; set; } = [];
    [IgnoreDataMember] public List<TimeSlot> CreatedTimeSlots { get; set; } = [];
    [IgnoreDataMember] public List<University> CreatedUniversities { get; set; } = [];
}


public enum GenderType
{
    [Display(Name = "زن")]
    Female,
    [Display(Name = "مرد")]
    Male
}

public class UserInfo : IBaseEntity<long> 
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ConnectCode { get; set; }
    public GenderType Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTimeOffset CreateDate { get; set; }
    public long? PartnerId { get; set; }
    [IgnoreDataMember] public User User { get; set; } = null!;
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(user => user.UserName).IsRequired().HasMaxLength(100);
        builder.HasMany(u => u.UserGroups)
            .WithMany(r => r.Users);
        builder.HasOne(u => u.Info)
            .WithOne(i => i.User)
            .HasForeignKey<UserInfo>(i => i.UserId);
        builder.HasMany(u => u.ApiTokens)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId);
        builder.HasMany(u => u.CalendarEvents)
            .WithMany(a => a.Users);

        builder.HasMany(i => i.ReceivedSms)
            .WithOne(i => i.ReceiverUser)
            .HasForeignKey(i => i.ReceiverUserId);
        builder.HasMany(i => i.Notifications)
            .WithOne(i => i.User)
            .HasForeignKey(i => i.UserId);
        builder.HasMany(i => i.CreatedCalendarEvents)
            .WithOne(i => i.CreatorUser)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.CreatedUserGroups)
            .WithOne(i => i.CreatorUser)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.CreatedAssignments)
            .WithOne(i => i.CreatorUser)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.CreatedCourseOfferings)
            .WithOne(i => i.CreatorUser)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.CreatedCourses)
            .WithOne(i => i.CreatorUser)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.CreatedDegreeLevels)
            .WithOne(i => i.CreatorUser)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.CreatedFields)
            .WithOne(i => i.CreatorUser)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.CreatedProfessors)
            .WithOne(i => i.CreatorUser)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.CreatedProfessorAvailabilities)
            .WithOne(i => i.CreatorUser)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.CreatedProfessorPresences)
            .WithOne(i => i.CreatorUser)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.CreatedProfessorSkills)
            .WithOne(i => i.CreatorUser)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.CreatedRooms)
            .WithOne(i => i.CreatorUser)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.CreatedTerms)
            .WithOne(i => i.CreatorUser)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.CreatedTimeSlots)
            .WithOne(i => i.CreatorUser)
            .HasForeignKey(i => i.CreatorUserId);
        builder.HasMany(i => i.CreatedUniversities)
            .WithOne(i => i.CreatorUser)
            .HasForeignKey(i => i.CreatorUserId);
        
    }
}

public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
{
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder.Property(i => i.Address).HasDefaultValue("");
        builder.HasOne(i => i.User)
            .WithOne(u => u.Info)
            .HasForeignKey<UserInfo>(i => i.UserId);
    }
}