#region

using ISO3166CZ;
using RegisterMe.Application.CatRegistrations.Dtos;

#endregion

namespace RegisterMe.Application.CatRegistrations.Commands.Validators;

public class CreateBreederCommandValidator : AbstractValidator<BreederDto>
{
    public CreateBreederCommandValidator()
    {
        List<string> supportedCOdes = Country.GetCountries()
            .Select(x => x.Alpha2.ToString()).ToList();

        RuleFor(v => v.Country).NotEmpty().MaximumLength(2).Must(x => supportedCOdes.Contains(x));
        RuleFor(x => x.FirstName).NotEmpty().Length(1, 75);
        RuleFor(x => x.LastName).NotEmpty().Length(1, 75);
    }
}
