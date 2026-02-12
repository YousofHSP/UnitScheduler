using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Data;
using Microsoft.EntityFrameworkCore;
using Service.DataInitializer;

namespace WebFramework.Configuration;

public static class ApplicationBuilderException
{
    public static void UseHsts(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            app.UseHsts();
        }
    }

    public static async Task DataSeeder(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>()!; // service locator
        await dbContext.Database.EnsureCreatedAsync();
        await dbContext.Database.MigrateAsync();

        // var dataInitializerAssembly = typeof(IDataInitializer).Assembly;
        // var dataInitializers = dataInitializerAssembly.GetTypes()
        //     .Where(i => i.IsClass && typeof(IDataInitializer).IsAssignableFrom(i));
        // foreach (var dataInitializer in dataInitializers)
        //     if (Activator.CreateInstance(dataInitializer) is IDataInitializer instance) instance.InitializerData();
        var dataInitializers = scope.ServiceProvider.GetServices<IDataInitializer>();
        foreach (var item in dataInitializers)
            await item.InitializerData();
    }
}