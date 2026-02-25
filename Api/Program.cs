using Common;
using Common.Utilities;
using Data.Contracts;
using Data.Repositories;
using Domain.Auth;
using Domain.DTOs.CustomMapping;
using Domain.Message;
using Domain.Model;
using Microsoft.AspNetCore.Mvc.Authorization;
using NLog;
using NLog.Web;
using Scalar.AspNetCore;
using Service.Auth;
using Service.DataInitializer;
using Service.Engine;
using Service.Engine.Contract;
using Service.Message;
using Service.Model;
using Service.Model.Contracts;
using Service.Reports;
using Service.Reports.Contracts;
using Shared;
using WebFramework.Filters;
using WebFramework.OpenApi;
using WebFramework.Configuration;
using WebFramework.Middlewares;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
try
{
    logger.Info("Starting web api ...");
    var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
    var siteSettings = builder.Configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
    builder.Services.Configure<SiteSettings>(builder.Configuration.GetSection(nameof(SiteSettings)));
    builder.Services.AddScoped<LogActionExecutionAttribute>();
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add(new AuthorizeFilter());
        options.Filters.Add<LogActionExecutionAttribute>();
    });
    
    builder.Services.AddDbContext(builder.Configuration);
    builder.Services.AddMemoryCache();
    builder.Services.AddCustomIdentity(siteSettings.IdentitySettings);
    
    
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<IExcelExport, ExcelExport>();
    builder.Services.AddScoped<IWordReportService, WordReportService>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddScoped<IOtpService, OtpService>();
    builder.Services.AddScoped<ISettingService, SettingService>();
    builder.Services.AddScoped<IUploadedFileService, UploadedFileService>();
    builder.Services.AddScoped<IEmailService, SmtpEmailService>();
    builder.Services.AddScoped<IMessageService, KavehNegarMessageService>();
    builder.Services.AddScoped<INotificationService, NotificationService>();
    builder.Services.AddScoped<IEngineService, EngineService>();
    
    
    builder.Services.AddScoped<IDataInitializer, CityDataInitializer>();
    builder.Services.AddScoped<IDataInitializer, RoleDataInitializer>();
    builder.Services.AddScoped<IDataInitializer, UserDataInitializer>();
    builder.Services.AddScoped<IDataInitializer, SettingDataInitializer>();
    
    
    builder.Services.AddJwtAuthentication();
    
    builder.Logging.ClearProviders();
    builder.Host.UseNLog(new NLogAspNetCoreOptions
    {
        CaptureMessageTemplates = true,
        CaptureMessageProperties = true,
        IncludeScopes = true
    });

    builder.Services.InitializeAutoMapper();
    builder.Services.AddCustomApiVersioning();
    builder.Services.AddCorsService();
    builder.Services.AddSignalR();
    
    builder.Services.AddHttpClient("KavehNegar", client =>
    {
        client.BaseAddress = new Uri("https://api.kavenegar.com/v1/");
    });

    builder.Services.AddOpenApiCustome();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "Api";
            options.AddDocument("v1");
        });
    }
    
    app.Use(async (context, next) =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "";
        var userAgent = context.Request.Headers["User-Agent"].ToString();
        var requestId = context.TraceIdentifier;
        var physicalPath = context?.Request.Path ?? "";
    
        ScopeContext.PushProperty("ipAddress", ip);
        ScopeContext.PushProperty("userAgent", userAgent);
        ScopeContext.PushProperty("requestId", requestId);
        ScopeContext.PushProperty("physicalPath", physicalPath);
    
    
        await next();
    });

    app.UseHsts(app.Environment);
    await app.DataSeeder(app.Environment);
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.Use(async (context, next) =>
    {
        var userName = context.User.Identity.IsAuthenticated
            ? context.User.Identity.GetUserName()
            : "Anonymous";
    
        ScopeContext.PushProperty("userName", userName);
    
    
        await next();
    });
    app.MapControllers();

    app.UseCors("AllowAll");
    app.UseCustomExceptionHandler();

    app.Run();

}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}