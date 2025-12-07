using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.Extensions.FileProviders;
using OmniRepo.Infrastructure.Data;
using OmniRepo.Web;
using OmniRepo.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.AddApplicationServices();

builder.AddInfrastructureServices();
builder.AddWebServices();

builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
    // Don't give bots any ideas...
    serverOptions.AddServerHeader = false;
    var config = context.Configuration.GetSection("Kestrel");
    
    serverOptions.Configure(config);
    
    if (!config.GetChildren().Any() && string.IsNullOrEmpty(context.Configuration["ASPNETCORE_URLS"]))
    {
        serverOptions.ListenAnyIP(5333); 
    }
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // await app.InitialiseDatabaseAsync();
    app.UseWebAssemblyDebugging();

}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseDirectoryBrowser(options: new DirectoryBrowserOptions()
{
    RequestPath = new PathString("/static"),
    // RedirectToAppendTrailingSlash =  true
});
app.UseHealthChecks("/health");


app.UseAntiforgery();


app.UseOpenApi(settings =>
{
    settings.Path = "/api/specification.json";
});
// app.UseStatusCodePagesWithReExecute(
//     "/404", createScopeForStatusCodePages: true);

app.UseReDoc(settings =>
{
    settings.Path = "/api";
    settings.DocumentTitle = "Feel the weight of what we owe";
    settings.DocumentPath = "/api/specification.json";
});
// app.UseAuthentication();
// app.UseAuthorization();
app.MapStaticAssets();
app.MapEndpoints();
// app.MapControllers();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
// app.MapBlazorHub();

// app.MapFallbackToPage("/_Host"); // Required for Blazor Server to map the root page


app.Run();

public partial class Program { }
