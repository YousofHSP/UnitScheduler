namespace WebClient.Components.Shared.DataTableComponent;

/// <summary>پارامترهایی که کامپوننت برای هر بار گرفتن داده به سرور می‌فرستد.</summary>
public class DataTableRequest
{
    public int Page { get; set; } = 1;          // از 1 شروع می‌شود
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public string? SortBy { get; set; }         // مقدار Field ستون
    public bool SortDescending { get; set; }

    /// <summary>کلید = Field ستون، مقدار = مقدار فیلتر (فقط فیلترهای پُر).</summary>
    public Dictionary<string, string> Filters { get; set; } = new();

    public int Skip => (Page - 1) * PageSize;
}

/// <summary>نتیجهٔ سمت سرور.</summary>
public class DataTableResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int TotalCount { get; set; }
}
