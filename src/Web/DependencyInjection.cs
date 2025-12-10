using Aptabase.Core;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Mvc;
using OmniRepo.Web.Services;
using OpenTelemetry.Trace;
using Sentry.OpenTelemetry;


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
        // OpenTelemetry is required for the new .NET 10 Blazor telemetry features
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing.AddSource("Microsoft.AspNetCore.Components");
                tracing.AddSource("Microsoft.AspNetCore.Components.Server.Circuits");
                tracing.AddAspNetCoreInstrumentation();
                // Add Sentry as an exporter
                tracing.AddSentry();

            });
        builder.Services.AddSingleton<BlazorSentryIntegration>();
        builder.Services.AddHostedService(sp => sp.GetRequiredService<BlazorSentryIntegration>());
        builder.Services.AddScoped<CircuitHandler, SentryCircuitHandler>();
        builder.Services.AddSingleton(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<AptabaseClient>>();

            return new AptabaseClient(
                "A-US-5255704934",
                new AptabaseOptions
                {
                    EnablePersistence = true,
#if DEBUG
                    IsDebugMode = true
#else
            IsDebugMode = false
#endif
                },
                logger
            );
        });


    }
}
