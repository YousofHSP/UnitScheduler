using Common;
using Common.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared;
using System.Net;
using WebClient.Services.Components;
using Exception = System.Exception;

namespace WebClient.Services.Common;

public class BaseService : IBaseService
{
    private readonly HttpClient _httpClient;
    private readonly ToastService _toastService;
    private readonly IJSRuntime _jsRuntime;
    private readonly NavigationManager NavManager;

    public BaseService(HttpClient httpClient, ToastService toastService, IJSRuntime jsRuntime,
        NavigationManager navManager)
    {
        _httpClient = httpClient;
        _toastService = toastService;
        _jsRuntime = jsRuntime;
        NavManager = navManager;
    }

    public async Task<TResDto?> Get<TResDto>(string uri) 
    {
        try
        {
            await SetHeaders();
            var result = await _httpClient.GetAsync(uri);
            if (result.StatusCode is HttpStatusCode.InternalServerError)
                throw new Exception("خطا در دریافت اطلاعات");
            var response = await result.Content.ReadFromJsonAsync<ApiResultClient<TResDto>>();
            if (response is null)
                throw new Exception("خطا در تبدیل اطلاعات");
            if (response.StatusCode is ApiResultStatusCode.PaymentRequired)
                throw new LogicException(response.Message);
            if (!response.IsSuccess)
                throw new Exception(response.Message);
            return response.Data;
        }
        catch (LogicException e)
        {
            _toastService.ShowError(e.Message);
            NavManager.NavigateTo("/License", true);
            return default;
        }
        catch (Exception e)
        {
            _toastService.ShowError(e.Message);
            return default;
        }
    }

    public async Task<bool> Get(string uri)
    {
        try
        {
            await SetHeaders();
            var result = await _httpClient.GetAsync(uri);
            if (result.StatusCode is HttpStatusCode.InternalServerError)
                throw new Exception("خطا در دریافت اطلاعات");
            var response = await result.Content.ReadFromJsonAsync<ApiResultClient>();
            if (response is null)
                throw new Exception("خطا در تبدیل اطلاعات");
            if (response.StatusCode is ApiResultStatusCode.PaymentRequired)
                throw new LogicException(response.Message);
            if (!response.IsSuccess)
                throw new Exception(response.Message);
            return true;
        }
        catch (LogicException e)
        {
            _toastService.ShowError(e.Message);
            NavManager.NavigateTo("/License", true);
            return false;
        }
        catch (Exception e)
        {
            _toastService.ShowError(e.Message);
            return false;
        }
    }
    public async Task<byte[]?> PostFile<T>(string uri, T dto)
    {
        try
        {
            await SetHeaders();

            var result = await _httpClient.PostAsJsonAsync(uri, dto);

            if (result.StatusCode == HttpStatusCode.InternalServerError)
                throw new Exception("خطا در دریافت فایل");

            if (!result.IsSuccessStatusCode)
                throw new Exception("دانلود فایل با خطا مواجه شد");

            return await result.Content.ReadAsByteArrayAsync();
        }
        catch (LogicException e)
        {
            _toastService.ShowError(e.Message);
            NavManager.NavigateTo("/License", true);
            return null;
        }
        catch (Exception e)
        {
            _toastService.ShowError(e.Message);
            return null;
        }
    }

    public async Task<(string FileName, byte[] File)?> GetFileAndName(string uri)
    {
        try
        {
            await SetHeaders();

            var result = await _httpClient.GetAsync(uri);

            if (result.StatusCode is HttpStatusCode.InternalServerError)
                throw new Exception("خطا در دریافت فایل");

            if (!result.IsSuccessStatusCode)
                throw new Exception("دانلود فایل با خطا مواجه شد");

            var contentDisposition = result.Content.Headers.ContentDisposition;
            var fileName = contentDisposition?.FileNameStar ?? contentDisposition?.FileName ?? "file.bin";
            var file = await result.Content.ReadAsByteArrayAsync();
            return (FileName: fileName, File: file);
        }
        catch (LogicException e)
        {
            _toastService.ShowError(e.Message);
            NavManager.NavigateTo("/License", true);
            return null;
        }
        catch (Exception e)
        {
            _toastService.ShowError(e.Message);
            return null;
        }        
    }
    public async Task<byte[]?> GetFile(string uri)
    {
        try
        {
            await SetHeaders();

            var result = await _httpClient.GetAsync(uri);

            if (result.StatusCode is HttpStatusCode.InternalServerError)
                throw new Exception("خطا در دریافت فایل");

            if (!result.IsSuccessStatusCode)
                throw new Exception("دانلود فایل با خطا مواجه شد");

            return await result.Content.ReadAsByteArrayAsync();
        }
        catch (LogicException e)
        {
            _toastService.ShowError(e.Message);
            NavManager.NavigateTo("/License", true);
            return null;
        }
        catch (Exception e)
        {
            _toastService.ShowError(e.Message);
            return null;
        }
    }

    public async Task<bool> Delete(string uri)
    {
        try
        {
            await SetHeaders();
            var result = await _httpClient.DeleteAsync(uri);
            if (result.StatusCode is HttpStatusCode.InternalServerError)
                throw new Exception("خطا در دریافت اطلاعات");
            var response = await result.Content.ReadFromJsonAsync<ApiResultClient>();
            if (response is null)
                throw new Exception("خطا در تبدیل اطلاعات");
            if (response.StatusCode is ApiResultStatusCode.PaymentRequired)
                throw new LogicException(response.Message);
            if (!response.IsSuccess)
                throw new Exception(response.Message);
            return true;
        }
        catch (LogicException e)
        {
            _toastService.ShowError(e.Message);
            NavManager.NavigateTo("/License", true);
            return false;
        }
        catch (Exception e)
        {
            _toastService.ShowError(e.Message);
            return false;
        }
    }

    public HttpClient GetHttpClient()
        => _httpClient;

    public async Task<TResDto?> Post<TDto, TResDto>(string uri, TDto dto, bool checkAuth = true) 
    {
        try
        {
            await SetHeaders();

            HttpResponseMessage result;
            if (dto is MultipartFormDataContent content)
            {
                result = await _httpClient.PostAsync(uri, content);
            }
            else
            {
                result = await _httpClient.PostAsJsonAsync(uri, dto);
            }

            if (result.StatusCode is HttpStatusCode.InternalServerError)
                throw new Exception("خطا در دریافت اطلاعات");
            var response = await result.Content.ReadFromJsonAsync<ApiResultClient<TResDto>>();
            if (response is null)
                throw new Exception("خطا در تبدیل اطلاعات");
            if (response.StatusCode is ApiResultStatusCode.UnAuthorized)
                throw new UnauthorizedAccessException(response.Message);
            if (response.StatusCode is ApiResultStatusCode.PaymentRequired)
                throw new LogicException(response.Message);
            if (!response.IsSuccess)
                throw new Exception(response.Message);

            return response.Data;
        }
        catch (LogicException e)
        {
            _toastService.ShowError(e.Message);
            NavManager.NavigateTo("/License", true);
            return default;
        }
        catch (UnauthorizedAccessException e)
        {
            _toastService.ShowError(e.Message);
            var returnUrl = Uri.EscapeDataString(NavManager.Uri);
            NavManager.NavigateTo($"/Auth/Login?returnUrl={returnUrl}", true);
            return default;
        }
        catch (Exception e)
        {
            _toastService.ShowError(e.Message);
            return default;
        }
    }

    public async Task<TResDto?> Patch<TDto, TResDto>(string uri, TDto dto) 
    {
        try
        {
            await SetHeaders();
            var result = await _httpClient.PatchAsJsonAsync(uri, dto);

            if (result.StatusCode is HttpStatusCode.InternalServerError)
                throw new Exception("خطا در دریافت اطلاعات");
            var response = await result.Content.ReadFromJsonAsync<ApiResultClient<TResDto>>();
            if (response is null)
                throw new Exception("خطا در تبدیل اطلاعات");
            if (response.StatusCode is ApiResultStatusCode.UnAuthorized)
                throw new UnauthorizedAccessException(response.Message);
            if (response.StatusCode is ApiResultStatusCode.PaymentRequired)
                throw new LogicException(response.Message);
            if (!response.IsSuccess)
                throw new Exception(response.Message);

            return response.Data;
        }
        catch (UnauthorizedAccessException e)
        {
            _toastService.ShowError(e.Message);
            var returnUrl = Uri.EscapeDataString(NavManager.Uri);
            NavManager.NavigateTo($"/Auth/Login?returnUrl={returnUrl}", true);
            return default;
        }
        catch (Exception e)
        {
            _toastService.ShowError(e.Message);
            return default;
        }
    }

    private async Task SetHeaders()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwt_token");
        GetHttpClient().DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<bool> Post<TDto>(string uri, TDto dto, bool checkAuth = true)
    {
        try
        {
            await SetHeaders();

            HttpResponseMessage result;
            if (dto is MultipartFormDataContent content)
            {
                result = await _httpClient.PostAsync(uri, content);
            }
            else
            {
                result = await _httpClient.PostAsJsonAsync(uri, dto);
            }

            if (result.StatusCode is HttpStatusCode.InternalServerError)
                throw new Exception("خطا در دریافت اطلاعات");
            if (result.StatusCode is HttpStatusCode.UnsupportedMediaType)
                throw new Exception("ارسال این نوع فایل مجاز نیست.");
            if (result.StatusCode is HttpStatusCode.NotFound)
                throw new Exception("آدرس پیدا نشد");
            var response = await result.Content.ReadFromJsonAsync<ApiResultClient>();
            if (response is null)
                throw new Exception("خطا در تبدیل اطلاعات");
            if (response.StatusCode is ApiResultStatusCode.UnAuthorized)
                throw new UnauthorizedAccessException(response.Message);
            if (response.StatusCode is ApiResultStatusCode.PaymentRequired)
                throw new LogicException(response.Message);
            if (!response.IsSuccess)
                throw new Exception(response.Message);

            return true;
        }
        catch (LogicException e)
        {
            _toastService.ShowError(e.Message);
            NavManager.NavigateTo("/License", true);
            return false;
        }
        catch (UnauthorizedAccessException e)
        {
            _toastService.ShowError(e.Message);
            var returnUrl = Uri.EscapeDataString(NavManager.Uri);
            NavManager.NavigateTo($"/Auth/Login?returnUrl={returnUrl}", true);
            return false;
        }
        catch (Exception e)
        {
            _toastService.ShowError(e.Message);
            return false;
        }
    }

    public async Task<bool> Post(string uri, bool checkAuth = true)
    {
        try
        {
            await SetHeaders();
            var result = await _httpClient.PostAsync(uri, null);

            if (result.StatusCode is HttpStatusCode.InternalServerError)
                throw new Exception("خطا در دریافت اطلاعات");
            var response = await result.Content.ReadFromJsonAsync<ApiResultClient>();
            if (response is null)
                throw new Exception("خطا در تبدیل اطلاعات");
            if (response.StatusCode is ApiResultStatusCode.UnAuthorized)
                throw new UnauthorizedAccessException(response.Message);
            if (response.StatusCode is ApiResultStatusCode.PaymentRequired)
                throw new LogicException(response.Message);
            if (!response.IsSuccess)
                throw new Exception(response.Message);

            return true;
        }
        catch (LogicException e)
        {
            _toastService.ShowError(e.Message);
            NavManager.NavigateTo("/License", true);
            return false;
        }
        catch (UnauthorizedAccessException e)
        {
            _toastService.ShowError(e.Message);
            var returnUrl = Uri.EscapeDataString(NavManager.Uri);
            NavManager.NavigateTo($"/Auth/Login?returnUrl={returnUrl}", true);
            return false;
        }
        catch (Exception e)
        {
            _toastService.ShowError(e.Message);
            return false;
        }
    }
}