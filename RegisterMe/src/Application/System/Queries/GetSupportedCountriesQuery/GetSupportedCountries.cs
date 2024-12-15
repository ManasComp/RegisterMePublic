#region

using ISO3166CZ;
using RegisterMe.Application.System.Dtos;

#endregion

namespace RegisterMe.Application.System.Queries.GetSupportedCountriesQuery;

// ReSharper disable always UnusedType.Global
public record GetSupportedCountriesQuery : IRequest<List<CountryDto>>
{
}

public class
    GetSupportedCountriesQueryValidator : AbstractValidator<
    GetSupportedCountriesQuery>
{
}

public class GetSupportedCountriesQueryHandler : IRequestHandler<GetSupportedCountriesQuery, List<CountryDto>>
{
    public Task<List<CountryDto>> Handle(GetSupportedCountriesQuery request, CancellationToken cancellationToken)
    {
        // anyone can check what cats can be registered
        List<CountryDto> availableCountries = Country.GetCountries()
            .Select(x => new CountryDto { CountryName = x.Name, CountryCode = x.Alpha2.ToString() })
            .OrderBy(x => x.CountryName).ToList();
        return Task.FromResult(availableCountries);
    }
}
