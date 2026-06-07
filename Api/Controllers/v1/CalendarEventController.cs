using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Exceptions;
using Common.Utilities;
using Data.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Model.Contracts;
using Shared;
using Shared.DTOs;
using WebFramework.Api;

namespace Api.Controllers.v1;

/// <summary>
/// این کنترلر مدیریت رویدادهای تقویمی سیستم را بر عهده داشته و امکان
/// دریافت رویدادهای مرتبط با کاربر جاری و ثبت رویداد بر اساس گروه‌های کاربری را فراهم می‌کند.
/// </summary>
/// <param name="repository">مخزن داده مربوط به موجودیت رویدادهای تقویمی.</param>
/// <param name="requestUserGroupRepository">مخزن ارتباط بین درخواست و گروه‌های کاربری.</param>
/// <param name="calendarEventService">سرویس مدیریت منطق تجاری رویدادهای تقویمی.</param>
/// <param name="mapper">نگاشت‌گر AutoMapper برای تبدیل Entity و DTO.</param>
[ApiVersion("1")]
[Display(Name = "رویداد ها")]
public class CalendarEventController(
    IRepository<CalendarEvent> repository,
    ICalendarEventService calendarEventService,
    IMapper mapper) : BaseController
{
    /// <summary>
    /// لیست رویدادهای تقویمی مرتبط با کاربر جاری را بازیابی می‌کند.
    /// </summary>
    /// <param name="ct">توکن لغو عملیات ناهمزمان.</param>
    /// <returns>لیست رویدادهای قابل مشاهده برای کاربر.</returns>
    [HttpGet("[action]")]
    [Display(Name = "رویداد های کاربر")]
    public async Task<ApiResult<List<CalendarEventResDto>>> GetUserEvents(CancellationToken ct)
    {
        var userId = User.Identity!.GetUserId<long>();
        var list = await repository.TableNoTracking
            .Where(i => i.Users.Select(u => u.Id).Contains(userId))
            .ProjectTo<CalendarEventResDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return Ok(list);
    }
}
