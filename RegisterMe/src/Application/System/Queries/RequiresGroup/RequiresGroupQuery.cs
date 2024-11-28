#region

using RegisterMe.Application.Services.Ems;
using RegisterMe.Application.System.Queries.GetAllSupportedCats;

#endregion

namespace RegisterMe.Application.System.Queries.RequiresGroup;

// ReSharper disable always UnusedType.Global
public record RequiresGroupQuery : IRequest<bool>
{
    public required string? Ems { get; init; }
}

public class
    RequiresGroupQueryValidator : AbstractValidator<
    GetAllSupportedCatsQuery>
{
}

public class RequiresGroupQueryHandler : IRequestHandler<RequiresGroupQuery, bool>
{
    public Task<bool> Handle(RequiresGroupQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Ems))
        {
            return Task.FromResult(false);
        }

        bool requiresBool = EmsCode.RequiresGroup(request.Ems);
        return Task.FromResult(requiresBool);
    }
}
