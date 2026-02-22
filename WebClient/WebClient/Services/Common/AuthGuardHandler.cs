using System.Net;
using Microsoft.AspNetCore.Components;

namespace WebClient.Services.Common;

public class AuthGuardHandler : DelegatingHandler
{
    private readonly RequestGuard _guard;
    private readonly NavigationManager _nav;

    public AuthGuardHandler(RequestGuard guard, NavigationManager nav)
    {
        _guard = guard;
        _nav = nav;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // اگر auth نامعتبر شد، جلوی ادامه request رو بگیر
        // if (_guard.IsAuthInvalid)
        // {
        //     return new HttpResponseMessage(HttpStatusCode.Unauthorized)
        //     {
        //         RequestMessage = request
        //     };
        // }

        var response = await base.SendAsync(request, cancellationToken);

        // اگر API 401 داد، guard رو فعال کن و کاربر رو به login هدایت کن
        // if (response.StatusCode == HttpStatusCode.Unauthorized)
        // {
        //     _guard.InvalidateAuth();
        //     _nav.NavigateTo("/Auth/Login", true);
        // }

        return response;
    }
}