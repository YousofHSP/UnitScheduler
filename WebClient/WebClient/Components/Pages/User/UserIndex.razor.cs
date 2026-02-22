using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.DTOs;
using WebClient.Services.Common;
using WebClient.Services.Components;
using Timer = System.Timers.Timer;

namespace WebClient.Components.Pages.User;

/// <summary>
/// 95 - تجهیزاتی که برای انجام آزمایش یا تست لازم است
/// </summary>
public partial class UserIndex : ComponentBase
{
    private List<UserResDto> _list = [];
    private UserDto _data = new();
    private int _total;
    private bool _modalIsBusy;
    private bool _isBusy;
    private IndexDto _indexDto = new();
    private bool _isLoading = true;
    private string _modalTitle = "";
    private Timer? _debounceTimer;


    [Inject] public IBaseService BaseService { get; set; }
    [Inject] public IJSRuntime Js { get; set; }
    [Inject] public ToastService ToastService { get; set; }

    
    private void OnSearchChanged(ChangeEventArgs e)
    {
        _indexDto.Search = e.Value?.ToString() ?? "";

        // اگر تایمر قبلی وجود داشت ریست بشه
        _debounceTimer?.Stop();
        _debounceTimer?.Dispose();

        // یه تایمر جدید با 1 ثانیه تأخیر
        _debounceTimer = new Timer(500) { AutoReset = false };
        _debounceTimer.Elapsed += async (_, _) =>
        {
            // صدا زدن سرور
            await InvokeAsync(GetData);
        };
        _debounceTimer.Start();
    }
    private async Task GetData()
    {
        var result = await BaseService.Post<IndexDto, IndexResDto<UserResDto>>("v1/User/Index", _indexDto);
        if (result is not null)
        {
            _list = result.Data;
            _indexDto.Page = result.Page;
            _indexDto.Limit = result.Limit;
            _total = result.Total;
            _isLoading = false;
            StateHasChanged();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                await GetData();
            }
        }
        catch

        {
            Console.WriteLine("error");
        }
    }

    private async Task Delete()
    {
        await Js.InvokeVoidAsync("closeModal", "deleteModal");
        _isLoading = true;
        try
        {
            var deleteResult = await BaseService.Delete($"v1/User/{_data.Id}");
            if (deleteResult)
            {
                ToastService.ShowSuccess(" حذف شد");
                await GetData();
            }
        }
        finally
        {
            _data = new();
            _isLoading = false;
        }
    }

    private async Task ShowCreateModal()
    {
        _modalTitle = "ایجاد";
        _data = new();
        await Js.InvokeVoidAsync("openModal", "dataModal");
    }

    private async Task ShowEditModal(long id)
    {
        await Js.InvokeVoidAsync("openModal", "dataModal");
        _modalTitle = "ویرایش";
        _modalIsBusy = true;
        try
        {
            var result = await BaseService.Get<UserResDto>($"v1/User/{id}");
            if (result is not null)
            {
                _data.UserGroupIds = result.UserGroupIds;
                _data.BirthDate = result.BirthDate;
                _data.FullName= result.FullName;
                _data.Email = result.Email;
                _data.Enable = result.Enable;
                _data.PhoneNumber = result.PhoneNumber;
                _data.Id = result.Id;
                _modalIsBusy = false;
                StateHasChanged();
            }
        }
        finally
        {
            _modalIsBusy = false;
        }
    }

    private async Task ShowDeleteWarning(long id)
    {
        await Js.InvokeVoidAsync("openModal", "deleteModal");
        _data = new() { Id = id };
    }
    
    public async Task DoneForm()
    {
        ToastService.ShowSuccess("اطلاعات  با موفقیت ثبت شد");
        await Js.InvokeVoidAsync("closeModal", "dataModal");
        await GetData();
    }
    public async Task PageChanged((int Page, int Limit) args)
    {
        _indexDto.Page = args.Page;
        _indexDto.Limit = args.Limit;
        await GetData();
    }
}