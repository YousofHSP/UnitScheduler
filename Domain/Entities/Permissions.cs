namespace Domain.Entities;

public static class Permissions
{
    public static List<Permission> All =
    [
        
        new() { Controller = "Role", ControllerLabel = "نقش",  Action = "Index", ActionLabel = "نمایش" },
        new() { Controller = "Role", ControllerLabel = "نقش", Action = "Create",  ActionLabel = "ایجاد" },
        new() { Controller = "Role", ControllerLabel = "نقش", Action = "Update",  ActionLabel = "ویرایش" },
        new() { Controller = "Role", ControllerLabel = "نقش", Action = "Delete", ActionLabel = "حذف" },
        
        new() { Controller = "User", Action = "Index", ControllerLabel = "کاربر", ActionLabel = "نمایش" },
        new() { Controller = "User", Action = "Create", ControllerLabel = "کاربر", ActionLabel = "ایجاد" },
        new() { Controller = "User", Action = "Update", ControllerLabel = "کاربر", ActionLabel = "ویرایش" },
        new() { Controller = "User", Action = "Delete", ControllerLabel = "کاربر", ActionLabel = "حذف" },
        new() { Controller = "User", Action = "GetTokens", ControllerLabel = "کاربر", ActionLabel = "لیست نشست ها" },
        new() { Controller = "User", Action = "DisableTokens", ControllerLabel = "کاربر", ActionLabel = "غیرفعال کردن توکن" },
        
        new() { Controller = "Sms", Action = "Index", ControllerLabel = "پیام", ActionLabel = "لیست" },
        
        new() { Controller = "Course", Action = "Index", ControllerLabel = "درس", ActionLabel = "لیست" },
        new() { Controller = "Course", ControllerLabel = "درس", Action = "Create",  ActionLabel = "ایجاد" },
        new() { Controller = "Course", ControllerLabel = "درس", Action = "Update",  ActionLabel = "ویرایش" },
        new() { Controller = "Course", ControllerLabel = "درس", Action = "Delete", ActionLabel = "حذف" },

        new() { Controller = "Professor", ControllerLabel = "استاد", Action = "Index",  ActionLabel = "لیست" },
        new() { Controller = "Professor", ControllerLabel = "استاد", Action = "Create",  ActionLabel = "ایجاد" },
        new() { Controller = "Professor", ControllerLabel = "استاد", Action = "Update",  ActionLabel = "ویرایش" },
        new() { Controller = "Professor", ControllerLabel = "استاد", Action = "Delete", ActionLabel = "حذف" },
        
        new() { Controller = "University", ControllerLabel = "دانشگاه", Action = "Index",  ActionLabel = "لیست" },
        new() { Controller = "University", ControllerLabel = "دانشگاه", Action = "Create",  ActionLabel = "ایجاد" },
        new() { Controller = "University", ControllerLabel = "دانشگاه", Action = "Update",  ActionLabel = "ویرایش" },
        new() { Controller = "University", ControllerLabel = "دانشگاه", Action = "Delete", ActionLabel = "حذف" },
        
        new() { Controller = "Term", ControllerLabel = "ترم", Action = "Index",  ActionLabel = "لیست" },
        new() { Controller = "Term", ControllerLabel = "ترم", Action = "Create",  ActionLabel = "ایجاد" },
        new() { Controller = "Term", ControllerLabel = "ترم", Action = "Update",  ActionLabel = "ویرایش" },
        new() { Controller = "Term", ControllerLabel = "ترم", Action = "Delete", ActionLabel = "حذف" },
        
        new() { Controller = "Assignment", ControllerLabel = "تخصیص‌ها", Action = "Professors", ActionLabel = "اساتید" },
        
        /////////////////////////////////
        
    ];

    public static bool AreValidPermissions(IEnumerable<string> permissions)
    {
        var validPermissionStrings = All
            .Select(p => $"{p.Controller}.{p.Action}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return permissions.All(p => validPermissionStrings.Contains(p));
    }
}

public class Permission
{
    public required string Controller { get; set; }
    public required string ControllerLabel { get; set; }
    public required string Action { get; set; }
    public required string ActionLabel { get; set; }
}