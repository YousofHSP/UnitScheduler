namespace WebClient.Components.Shared;

/// <summary>
/// سرویس مشترک نمایش منوی عملیات (ActionMenu).
///
/// دلیل وجود این سرویس:
/// ----------------------
/// منوی عملیات نباید داخل کامپوننت‌هایی مانند جدول، مودال یا دیالوگ رندر شود،
/// زیرا بسیاری از مودال‌ها از CSS هایی مانند:
///
/// - transform
/// - translate
/// - scale
///
/// استفاده می‌کنند.
///
/// وجود این خصوصیات باعث می‌شود مختصات position:fixed یا position:absolute
/// نسبت به viewport محاسبه نشود و منو در محل اشتباه نمایش داده شود.
///
/// به همین دلیل ActionMenu فقط یک بار در ریشه برنامه (App/MainLayout)
/// رندر می‌شود و سایر کامپوننت‌ها از طریق این سرویس درخواست نمایش آن را
/// ارسال می‌کنند.
///
/// این سرویس فقط وظیفه برقراری ارتباط بین درخواست‌کننده و ActionMenuHost
/// را بر عهده دارد و هیچ منطق نمایشی در آن وجود ندارد.
/// </summary>
public class ActionMenuService
{
    public Func<
        double,
        double,
        Dictionary<string, string>,
        Func<string, Task>,
        Task>? Show;
}