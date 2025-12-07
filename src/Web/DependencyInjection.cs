using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using OmniRepo.Application.Common.Interfaces;
using OmniRepo.Web.Services;
using Microsoft.AspNetCore.Mvc;


namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        // builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        // builder.Services.AddScoped<IUser, CurrentUser>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHealthChecks();
            // .AddDbContextCheck<ApplicationDbContext>();
        
        builder.Services.AddExceptionHandler<CustomExceptionHandler>();
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApiDocument((configure, sp) =>
        {
            configure.Title = "OmniRepo API";
            configure.Description = "And we all lift... together";
            configure.UseControllerSummaryAsTagDescription = true;

        });
    }
}
