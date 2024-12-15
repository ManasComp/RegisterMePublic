#region

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Interfaces;
using RegisterMe.Domain.Entities;
using RegisterMe.Infrastructure.Data;
using RegisterMe.Infrastructure.Data.Interceptors;
using RegisterMe.Infrastructure.Identity;
using RegisterMe.Infrastructure.Services;
using Serilog;
using Serilog.Events;

#endregion

namespace RegisterMe.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        WebApplicationBuilder builder)
    {
        ConfigurationManager configuration = builder.Configuration;
        string? actualDir = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName;
        LoadEnvFile(builder, actualDir);

        configuration.AddEnvironmentVariables();
        services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
        string? connectionString = configuration.GetConnectionString("DefaultConnection");
        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<IEmailSender, EmailSender>();

        bool useMsql = configuration.GetValue("USE_MSSQL", false);
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options
                .ConfigureWarnings(warnings =>
                    warnings.Ignore(RelationalEventId.MultipleCollectionIncludeWarning));
            if (sp.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }

            if (useMsql)
            {
                options.UseSqlServer(connectionString, x => x.MigrationsAssembly("DAL.Migrations.SqlServer"));
            }
            else
            {
                options.UseNpgsql(connectionString, x => x.MigrationsAssembly("DAL.Migrations.Postgres"));
            }
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitializer>();

        services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        services.AddAuthorizationBuilder();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.User.RequireUniqueEmail = true;
        });

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        services.ConfigureApplicationCookie(options =>
        {
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.Cookie.Name = "RegisterMeAuth";
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(1);
            options.LoginPath = "/Identity/Account/Login";
            options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
            options.SlidingExpiration = true;
        });
        services.Configure<PasswordHasherOptions>(option =>
        {
            option.IterationCount = 12000;
        });
        services.Configure<SecurityStampValidatorOptions>(o =>
            o.ValidationInterval = TimeSpan.FromMinutes(30));

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IIdentityService, IdentityService>();

        services.AddDataProtection()
            .PersistKeysToDbContext<ApplicationDbContext>();

        services.AddSerilog(builder);
        return services;
    }

    private static void LoadEnvFile(WebApplicationBuilder builder, string? actualDir)
    {
        if (actualDir == null)
        {
            return;
        }

        string? root = Directory.GetParent(actualDir)?.FullName;
        if (root == null)
        {
            return;
        }

        string? dotenv = null;

        if (builder.Environment.IsEnvironment("Development"))
        {
            dotenv = Path.Combine(root, ".env.development");
        }
        else if (builder.Environment.IsEnvironment("Production"))
        {
            dotenv = Path.Combine(root, ".env.production");
        }

        if (dotenv != null)
        {
            DotEnv.Load(dotenv);
        }
    }

    public static IServiceCollection AddSerilog(this IServiceCollection _, WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            ConfigureSerilog(LogEventLevel.Verbose, LogEventLevel.Verbose);
        }
        else
        {
            ConfigureSerilog(LogEventLevel.Information, LogEventLevel.Warning);
        }

        return builder.Services;

        void ConfigureSerilog(LogEventLevel defaultLevel, LogEventLevel microsoftLevel)
        {
            builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
                .ReadFrom.Configuration(context.Configuration)
                .MinimumLevel.Is(defaultLevel)
                .MinimumLevel.Override("Microsoft", microsoftLevel)
                .WriteTo.Console()
                .WriteTo.Seq("http://localhost:5341")
                .WriteTo.Seq("http://seq:5341"));
        }
    }
}
