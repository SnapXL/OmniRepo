using OmniRepo.Application.Common.Interfaces;
using OmniRepo.Domain.Constants;
using OmniRepo.Infrastructure.Data;
// using OmniRepo.Infrastructure.Data.Interceptors;
using OmniRepo.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("OmniRepoDb");

        // builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        // builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        //
        // builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        // {
        //     options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        //     options.UseSqlServer(connectionString);
        //     options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        // });

        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        // builder.Services
        //     .AddDefaultIdentity<ApplicationUser>()
        //     .AddRoles<IdentityRole>()
        //     .AddEntityFrameworkStores<ApplicationDbContext>();
        // builder.Services.AddSingleton<IIdentityDbConnectionProvider<SqliteConnection>, IdentityDbConnectionProvider>();

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddUserStore<ApplicationUserStore>()
            .AddRoleStore<ApplicationRoleStore>()
            .AddDefaultTokenProviders();
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();

        builder.Services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));
    }
}
