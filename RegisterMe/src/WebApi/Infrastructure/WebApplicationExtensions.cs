#region

using System.Reflection;
using Asp.Versioning.Builder;

#endregion

namespace WebApi.Infrastructure;

public static class WebApplicationExtensions
{
    public static RouteGroupBuilder MapGroup(this WebApplication app, EndpointGroupBase group, string? groupName = null)
    {
        groupName ??= group.GetType().Name;

        ApiVersionSet apiVersionSet = app.NewApiVersionSet()
            .ReportApiVersions()
            .Build();

        return app
            .MapGroup("api/v{version:apiVersion}/" + groupName)
            .WithGroupName(groupName)
            .WithTags(groupName)
            .WithApiVersionSet(apiVersionSet)
            .WithOpenApi();
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        Type endpointGroupType = typeof(EndpointGroupBase);

        Assembly assembly = Assembly.GetExecutingAssembly();

        IEnumerable<Type> endpointGroupTypes = assembly.GetExportedTypes()
            .Where(t => t.IsSubclassOf(endpointGroupType));

        foreach (Type type in endpointGroupTypes)
        {
            if (Activator.CreateInstance(type) is EndpointGroupBase instance)
            {
                instance.Map(app);
            }
        }

        return app;
    }
}
