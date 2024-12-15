namespace WebApi.Infrastructure;

public static class EndpointRouteBuilderExtensions
{
    private static RouteHandlerBuilder Map(RouteHandlerBuilder route, Delegate handler)
    {
        Guard.Against.AnonymousMethod(handler);

        return route
            .WithName(handler.Method.Name)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static RouteHandlerBuilder MapGet(this IEndpointRouteBuilder builder, Delegate handler,
        string pattern = "")
    {
        RouteHandlerBuilder route = builder.MapGet(pattern, handler);
        return Map(route, handler);
    }

    public static RouteHandlerBuilder MapPost(this IEndpointRouteBuilder builder, Delegate handler,
        string pattern = "")
    {
        RouteHandlerBuilder route = builder.MapPost(pattern, handler);
        return Map(route, handler);
    }

    public static RouteHandlerBuilder MapPut(this IEndpointRouteBuilder builder, Delegate handler, string pattern)
    {
        RouteHandlerBuilder route = builder.MapPut(pattern, handler);
        return Map(route, handler);
    }

    public static RouteHandlerBuilder MapDelete(this IEndpointRouteBuilder builder, Delegate handler, string pattern)
    {
        RouteHandlerBuilder route = builder.MapDelete(pattern, handler);
        return Map(route, handler);
    }
}
