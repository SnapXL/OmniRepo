using OmniRepo.Web.Components;
using OmniRepo.Web.Services;
using Sentry.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.AddApplicationServices();

builder.AddInfrastructureServices();
builder.AddWebServices();

builder.WebHost.ConfigureKestrel(
    (context, serverOptions) =>
    {
        // Don't give bots any ideas...
        serverOptions.AddServerHeader = false;

        var config = context.Configuration.GetSection("Kestrel");

        serverOptions.Configure(config);

        if (
            !config.GetChildren().Any()
            && string.IsNullOrEmpty(context.Configuration["ASPNETCORE_URLS"])
        )
        {
            serverOptions.ListenAnyIP(5333);
        }
    }
);
builder.WebHost.UseSentry(options =>
{
#if !SENTRY_DSN_DEFINED_IN_ENV
    // A DSN is required. You can set here in code, in the SENTRY_DSN environment variable or in your appsettings.json
    // See https://docs.sentry.io/product/sentry-basics/dsn-explainer/
    options.Dsn = "https://e1dd4c0a1951ec66f8c93acbeb377918@o4504136997928960.ingest.us.sentry.io/4510511618785280"; ;
#endif
    options.UseOpenTelemetry();
    options.AddEventProcessor(new BlazorEventProcessor());
    options.TracesSampleRate = 1.0;
    options.Debug = false;
    options.AddProfilingIntegration(TimeSpan.FromMilliseconds(500));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // await app.InitialiseDatabaseAsync();
    // app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();

app.UseDirectoryBrowser(
    options: new DirectoryBrowserOptions()
    {
        RequestPath = new PathString("/static"),
        // RedirectToAppendTrailingSlash =  true
    }
);
app.UseHealthChecks("/health");


app.UseOpenApi(settings =>
{
    settings.Path = "/api/specification.json";
});


app.UseReDoc(settings =>
{
    settings.Path = "/api";
    settings.DocumentTitle = "Feel the weight of what we owe";
    settings.DocumentPath = "/api/specification.json";
});


// app.UseAuthentication();
app.UseAuthorization();
app.UseStatusCodePagesWithReExecute(
    "/404", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapEndpoints();

// app.MapControllers();


app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// app.MapFallbackToPage("/_Host"); // Required for Blazor Server to map the root page

app.Run();

public partial class Program { }
