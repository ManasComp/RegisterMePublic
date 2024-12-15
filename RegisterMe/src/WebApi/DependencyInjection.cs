#region

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Infrastructure.Data;
using WebApi.Infrastructure;
using ZymLabs.NSwag.FluentValidation;

#endregion

// Do not move this to a different namespace
namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        services.AddRazorPages();

        services.AddScoped(provider =>
        {
            IEnumerable<FluentValidationRule>? validationRules =
                provider.GetService<IEnumerable<FluentValidationRule>>();
            ILoggerFactory? loggerFactory = provider.GetService<ILoggerFactory>();

            return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
        });

        // https://stackoverflow.com/questions/51218477/modelstate-from-actionfilter-asp-net-core-2-1-api
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddEndpointsApiExplorer();

        services.AddOpenApiDocument((configure, sp) =>
        {
            configure.Title = "RegisterMe API";
            configure.Description = "API for RegisterMe";

            FluentValidationSchemaProcessor fluentValidationSchemaProcessor =
                sp.CreateScope().ServiceProvider.GetRequiredService<FluentValidationSchemaProcessor>();

            configure.SchemaSettings.SchemaProcessors.Add(fluentValidationSchemaProcessor);
        });

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddExceptionHandler<CustomExceptionHandler>();

        return services;
    }
}
