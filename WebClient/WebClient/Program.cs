using Domain.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Server.Circuits;
using WebClient.Client.Pages;
using WebClient.Components;
using WebClient.Services;
using WebClient.Services.Api;
using WebClient.Services.Common;
using WebClient.Services.Components;
using WebClient.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>();

builder.Services.AddHttpContextAccessor();
// Component Service
builder.Services.AddScoped<ToastService>();
builder.Services.AddSingleton<CircuitHandler, TrackingCircuitHandler>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddControllers();
builder.Services.AddScoped<RequestGuard>();
builder.Services.AddHttpClient<IBaseService, BaseService>(client =>
{
    client.BaseAddress = new Uri(apiSettings.BaseUrl);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
});
// }).AddHttpMessageHandler<AuthGuardHandler>();
builder.Services.AddScoped<AuthGuardHandler>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/login";
        options.LogoutPath = "/logout";

        options.Cookie.Name = "__Host-BlazorAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;

        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// builder.Services.AddAuthorization();
// builder.Services.AddAuthentication(options => 
// {
//     options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
// }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
// {
//     options.LoginPath = "/Auth/Login";
//     options.Cookie.HttpOnly = true;
//     options.Cookie.SameSite = SameSiteMode.Lax;
//     options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//     options.SlidingExpiration = true;
//     options.ExpireTimeSpan = TimeSpan.FromDays(30);
// });
builder.Services.AddAuthorization(options =>
{
    foreach (var per in Permissions.All.Select(p => $"{p.Controller}.{p.Action}"))
        options.AddPolicy(per, policy => policy.RequireClaim("permission", per));
});
// builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthStateProvider>();
builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 1024 * 1024 * 20;
});
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(op => { op.DetailedErrors = true; });
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
// builder.Services.AddScoped<ISchedulingFrontendService, MockSchedulingFrontendService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(WebClient.Client._Imports).Assembly);

app.Run();