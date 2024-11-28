#region

using RegisterMe.Application.Services.Ems;

#endregion

namespace RegisterMe.Application.System.Queries.GetAllSupportedCats;

// ReSharper disable always UnusedType.Global
public record GetAllSupportedCatsQuery : IRequest<List<TypeOfCat>>
{
}

public class
    GetAllSupportedCatsQueryValidator : AbstractValidator<
    GetAllSupportedCatsQuery>
{
}

public class GetAllSupportedCatsQueryHandler : IRequestHandler<GetAllSupportedCatsQuery, List<TypeOfCat>>
{
    public Task<List<TypeOfCat>> Handle(GetAllSupportedCatsQuery request, CancellationToken cancellationToken)
    {
        // anyone can check what cats can be registered
        return Task.FromResult(EmsInitializer.Initialize().ToList());
    }
}
