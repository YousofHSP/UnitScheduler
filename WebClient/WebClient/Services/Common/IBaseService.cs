namespace WebClient.Services.Common;

public interface IBaseService
{
    Task<TResDto?> Post<TDto, TResDto>(string uri, TDto dto, bool checkAuth = true);
    Task<bool> Post<TDto>(string uri, TDto dto, bool checkAuth = true);
    Task<bool> Post(string uri, bool checkAuth = true);
    Task<TResDto?> Patch<TDto, TResDto>(string uri, TDto dto);
    Task<TResDto?> Get<TResDto>(string uri);
    Task<bool> Get(string uri);
    Task<(string FileName, byte[] File)?> GetFileAndName(string uri);
    Task<byte[]?> GetFile(string uri);
    Task<bool> Delete(string uri);
    HttpClient GetHttpClient();
    Task<byte[]?> PostFile<T>(string uri, T dto);
}