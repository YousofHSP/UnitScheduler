namespace Shared;

public class MenuGroup
{
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public List<MenuItem> Items { get; set; } = [];
}

public class MenuItem
{
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Policy { get; set; }
}

public static class MenuData
{
    public static List<MenuGroup> GetMenu() =>
    [

        new()
        {
            Title = "امنیت و کاربران",
            Icon = "users",
            Items =
            {
                new MenuItem { Policy = "Role.Index", Title = "نقش‌ها", Icon = "shield-check", Url = "Role/Index" },
                new MenuItem { Policy = "UserGroup.Index", Title = "گروه کاری", Icon = "user-group", Url = "UserGroup/Index" },
                new MenuItem { Policy = "User.Index", Title = "کاربران", Icon = "user", Url = "User/Index" },
                new MenuItem { Policy = "System.BackupsList", Title = "پشتیبان‌گیری", Icon = "archive-box-arrow-down", Url = "Backup/Index" },
                new MenuItem { Policy = "Log.ArchiveLogsIndex", Title = "آرشیوها", Icon = "archive-box", Url = "Log/Archive" },
                new MenuItem { Policy = "Log.AuditIndex", Title = "ممیزی", Icon = "clipboard-document-list", Url = "Log/Audit" },
                new MenuItem { Policy = "System.AuditsCheckIndex", Title = "بررسی ممیزی‌ها", Icon = "clipboard-document-check", Url = "System/AuditsCheck" },
                new MenuItem { Policy = "Log.LogIndex", Title = "لاگ ها", Icon = "list-bullet", Url = "Log/Index" },
                new MenuItem { Policy = "Sms.Index", Title = "پیام ها", Icon = "envelope", Url = "Sms/Index" },
                new MenuItem { Policy = "Setting.Index", Title = "تنظیمات", Icon = "adjustments-horizontal", Url = "Setting/Index" },
                // new MenuItem { Policy = "SubSystem.Index", Title = "زیرسیستم", Icon = "building-office-2", Url = "SubSystem/Index" },
                new MenuItem { Policy = "User.GetTokens", Title = "نشست ها", Icon = "", Url = "Sessions/Index" },
            }
        }
    ];
}