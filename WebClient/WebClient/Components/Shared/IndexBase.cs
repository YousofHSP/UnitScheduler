using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WebClient.Services.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using Shared.DTOs;
using WebClient.Services.Components;

namespace WebClient.Components.Shared;

public abstract class IndexBase<TDto, TResDto> : ComponentBase
{
    [Inject] public ToastService ToastService { get; set; } = null!;
    [Inject] public IJSRuntime Js { get; set; } = null!;
    [Inject] public IBaseService BaseService { get; set; } = null!;

    protected readonly IndexDto IndexDto = new();

    protected IndexResDto<TResDto> Data = new();
    public bool _isLoading = true;
    public string _modalTitle = "";
    public string ModelName { get; set; } = "";

    protected virtual async Task GetData()
    {
        
        _isLoading = true;
        Data = new();
        var result = await BaseService.Post<IndexDto, IndexResDto<TResDto>>($"v1/{ModelName}/Index", IndexDto);
        if (result is not null)
        {
            Data = result;
            _isLoading = false;
            StateHasChanged();
        }
    }

    protected async Task Reload(IndexDto dto)
    {
        IndexDto.Page = dto.Page;
        IndexDto.Limit = dto.Limit;
        IndexDto.Sort = dto.Sort;

        await GetData();
    }
    #region access
    [CascadingParameter] Task<AuthenticationState> AuthState { get; set; }
    public bool CanUpdate;
    public bool CanDelete;
    [Parameter] public bool HasUpdate { get; set; } = true;
    [Parameter] public bool HasDelete { get; set; } = true;
    public bool ShowOperation => CanUpdate || CanDelete;
    [Inject] public IAuthorizationService AuthorizationService { get; set; } = null!;

    protected override async Task OnParametersSetAsync()
    {
        var user = (await AuthState).User;
        if (HasUpdate)
        {
            CanUpdate = await AuthorizationService.AuthorizeAsync(
                user,
                $"{ModelName}.Update"
            ) is var editResult && editResult.Succeeded;
        }

        if (HasDelete)
        {
            CanDelete = await AuthorizationService.AuthorizeAsync(
                user,
                $"{ModelName}.Delete"
            ) is var deleteResult && deleteResult.Succeeded;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetData();
        }
    }

    #endregion

    public async Task HeaderClicked(string column)
    {
        // _isLoading = true;
        if (IndexDto.Sort.By == column)
            IndexDto.Sort.IsAscending = !IndexDto.Sort.IsAscending;
        else
        {
            IndexDto.Sort.By = column;
            IndexDto.Sort.IsAscending = true;
        }

        IndexDto.Page = 1;
        await GetData();
        _isLoading = false;
    }

    public async Task HandleSearch(string? text)
    {
        IndexDto.Search = text ?? "";
        IndexDto.Page = 1;
        await InvokeAsync(GetData);
    }

    public async Task PageChanged((int Page, int Limit) args)
    {
        IndexDto.Page = args.Page;
        IndexDto.Limit = args.Limit;
        await GetData();
    }

    public async Task DoneForm()
    {
        ToastService.ShowSuccess("اطلاعات  با موفقیت ثبت شد");
        await Js.InvokeVoidAsync("closeModal", "dataModal");
        await GetData();
    }

}