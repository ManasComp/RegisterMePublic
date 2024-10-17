#region

using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.FunctionalTests.DataGenerators;
using RegisterMe.Application.FunctionalTests.Enums;
using RegisterMe.Domain.Constants;
using RegisterMe.Domain.Entities;
using RegisterMe.Infrastructure.Data;
using RegisterMe.Infrastructure.Identity;

#endregion

namespace RegisterMe.Application.FunctionalTests;

[SetUpFixture]
public class Testing
{
    private static ITestDatabase s_database = null!;
#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
    private static CustomWebApplicationFactory s_factory = null!;
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method

    private static IServiceScopeFactory s_scopeFactory = null!;
    private static string? s_userId;
    private static DatabaseTypes? s_databaseType;

    private static readonly string DefaultPassword = "Admin123*";
    public static bool WasRun { get; private set; }

    public static void SetDatabaseType(DatabaseTypes databaseType)
    {
        s_databaseType = databaseType;
    }

    public static async Task RunBeforeAnyTests()
    {
        if (WasRun)
        {
            return;
        }

        WasRun = true;
        Guard.Against.Null(s_databaseType, nameof(s_databaseType));

        s_database = s_databaseType.Value switch
        {
            DatabaseTypes.Postgres => await TestDatabaseFactory.CreatePostgresAsync(),
            DatabaseTypes.SqlServer => await TestDatabaseFactory.CreateSqlServerAsync(),
            DatabaseTypes.DockerPostgres => await TestDatabaseFactory.CreateDockerPostgresServerAsync(),
            DatabaseTypes.DockerSqlServer => await TestDatabaseFactory.CreateDockerSqlServerAsync(),
            _ => throw new InvalidOperationException("Unsupported database type")
        };

        s_factory = new CustomWebApplicationFactory(s_database.GetConnection());
        s_scopeFactory = s_factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using IServiceScope scope = s_scopeFactory.CreateScope();
        ISender mediator = scope.ServiceProvider.GetRequiredService<ISender>();
        return await mediator.Send(request);
    }

    public static async Task SendAsync(IBaseRequest request)
    {
        using IServiceScope scope = s_scopeFactory.CreateScope();
        ISender mediator = scope.ServiceProvider.GetRequiredService<ISender>();
        await mediator.Send(request);
    }

    public static IServiceScopeFactory GetScopeFactory()
    {
        return s_scopeFactory;
    }

    public static string? GetUserId()
    {
        return s_userId;
    }

    public static ClaimsPrincipal GetClaimsPrincipal()
    {
        ClaimsPrincipal identity = new();
        return identity;
    }

    public static async Task<string?> RunAsExecutor(RunAsSpecificUser runAsSpecificUser)
    {
        return runAsSpecificUser switch
        {
            RunAsSpecificUser.RunAsOndrej => await RunAsOndrejAsync(),
            RunAsSpecificUser.RunAsSabrina => await RunAsSabrinaAsync(),
            RunAsSpecificUser.RunAsVojta => await RunAsVojtaAsync(),
            RunAsSpecificUser.RunAsAdministratorAsync => await RunAsAdministratorAsync(),
            RunAsSpecificUser.RunAsAnonymous => RunAsAnonymousUser(),
            _ => throw new Exception("User not found")
        };
    }

    public static async Task<string> RunAsOndrejAsync()
    {
        ApplicationUser user = UsersGenerator.Ondrej();
        return await RunAsUserAsync(user, DefaultPassword, []);
    }

    public static async Task<string> RunAsVojtaAsync()
    {
        ApplicationUser user = UsersGenerator.Vojta();
        return await RunAsUserAsync(user, DefaultPassword, []);
    }

    public static async Task<string> RunAsSabrinaAsync()
    {
        ApplicationUser user = UsersGenerator.Sabrina();
        return await RunAsUserAsync(user, DefaultPassword, []);
    }

    public static async Task<string> RunAsAdministratorAsync()
    {
        ApplicationUser admin = UsersGenerator.Admin();
        return await RunAsUserAsync(admin, DefaultPassword, [Roles.Administrator]);
    }

    public static async Task<string> RunAsUserAsync(ApplicationUser applicationUser, string password, string[] roles)
    {
        using IServiceScope scope = s_scopeFactory.CreateScope();
        UserManager<ApplicationUser> userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (applicationUser.Email != null && await userManager.Users.AnyAsync(x => x.Email == applicationUser.Email))
        {
            ApplicationUser? actualUser = await userManager.FindByEmailAsync(applicationUser.Email);
            s_userId = actualUser?.Id ?? throw new Exception("User not found");
            return s_userId;
        }

        IdentityResult result = await userManager.CreateAsync(applicationUser, password);
        if (roles.Length != 0)
        {
            RoleManager<IdentityRole> roleManager =
                scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (string role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            await userManager.AddToRolesAsync(applicationUser, roles);
        }

        if (result.Succeeded)
        {
            s_userId = applicationUser.Id;
            return s_userId;
        }

        string errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);
        throw new Exception($"Unable to create {applicationUser.UserName}.{Environment.NewLine}{errors}");
    }

    public static string? RunAsAnonymousUser()
    {
        s_userId = null;
        return s_userId;
    }

    public static async Task ResetState()
    {
        await s_database.ResetAsync();
        s_userId = null;
        using IServiceScope scope = s_scopeFactory.CreateScope();

        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        ApplicationUser user = UsersGenerator.Admin1();

        context.Users.Add(user);

        await context.SaveChangesAsync();

        ApplicationDbContextInitializer initializer =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

        await initializer.SeedTestExhibition(user, context, new TimeProviderMock());

        await context.SaveChangesAsync();
    }

    public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using IServiceScope scope = s_scopeFactory.CreateScope();

        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using IServiceScope scope = s_scopeFactory.CreateScope();

        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }

    public static IApplicationDbContext GetContext()
    {
        using IServiceScope scope = s_scopeFactory.CreateScope();

        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return context;
    }

    public static IMapper GetMapper()
    {
        using IServiceScope scope = s_scopeFactory.CreateScope();

        IMapper mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

        return mapper;
    }


    public static async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using IServiceScope scope = s_scopeFactory.CreateScope();

        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Set<TEntity>().CountAsync();
    }

    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        await s_database.DisposeAsync();
        await s_factory.DisposeAsync();
    }
}
