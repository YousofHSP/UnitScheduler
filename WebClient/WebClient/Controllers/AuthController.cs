using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebClient.Services.Api;

namespace WebClient.Controllers;

[Route("auth")]
[ApiController]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("signin")]
[AllowAnonymous]
    public async Task<IActionResult> SignIn([FromForm] string jwtToken, [FromForm] string? claims = null)
    {
        claims ??= "";
        var permissions = claims.Split(',').ToList();
        // await _authService.SignInAsync(jwtToken, permissions);
        return LocalRedirect("/");
    }

    [HttpGet("signout")]
    public async Task<IActionResult> SignOut()
    {
        await _authService.SignOutAsync();
        return LocalRedirect("/Auth/Login");
    }
}

public class SignInDto
{
    public string jwtToken { get; set; }
}