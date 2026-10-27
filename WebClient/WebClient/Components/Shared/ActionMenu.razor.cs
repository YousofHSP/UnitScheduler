using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WebClient.Components.Shared;

public partial class ActionMenu
{
    [Parameter]
    public Dictionary<string, string> Items { get; set; } = [];

    [Parameter]
    public EventCallback<string> OnSelect { get; set; }

    private bool IsOpen;

    private double Top;
    private double Left;
    /// <summary>
    /// باز کردن منو در مختصات مشخص.
    ///
    /// x و y مختصات Viewport هستند
    /// (MouseEventArgs.ClientX / ClientY).
    ///
    /// در صورت نزدیک بودن منو به لبه‌های صفحه،
    /// مختصات اصلاح می‌شود تا منو از Viewport خارج نشود.
    /// </summary>
    public async Task Open(double x, double y)
    {
        const int menuWidth = 180;
        const int menuHeight = 250;

        var viewportWidth = await Js.InvokeAsync<int>("eval", "window.innerWidth");
        var viewportHeight = await Js.InvokeAsync<int>("eval", "window.innerHeight");

        Left = x;
        Top = y;

        if (Left + menuWidth > viewportWidth)
            Left = viewportWidth - menuWidth - 10;

        if (Top + menuHeight > viewportHeight)
            Top = viewportHeight - menuHeight - 10;

        if (Left < 10)
            Left = 10;

        if (Top < 10)
            Top = 10;

        IsOpen = true;

        StateHasChanged();
    }

    private async Task ItemClicked(string key)
    {
        IsOpen = false;
        await OnSelect.InvokeAsync(key);
    }

    private void Close()
    {
        IsOpen = false;
    }
    private string GetIcon(string key)
    {
        return key switch
        {
            "Edit" => "✏️",
            "Show" => "👁️",
            "Detail" => "📄",
            "Download" => "⬇️",
            "Approve" => "✅",
            "Reject" => "❌",
            "Delete" => "🗑️",

            "CustomerContacts" => "👥",
            "AddCustomerContact" => "➕👤",

            "SubjectStatuses" => "📋",
            "SubjectUsers" => "👤",

            "History" => "🕒",
            "Note" => "📝",

            "detail" => "📄",
            "download" => "⬇️",
            "approve" => "✅",
            "reject" => "❌",

            "Diagram" => "📊",
            "Group" => "🗂️",
            "Settings" => "⚙️",

            _ => "🔹"
        };
    }

    [Inject]
    IJSRuntime Js { get; set; } = null!;

}