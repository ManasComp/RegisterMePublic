#region

using ISO3166CZ;
using RegisterMe.Application.Exhibitors.Dtos;

#endregion

namespace RegisterMe.Application.Exhibitors.Validators;

public class BaseExhibitor
{
    public abstract class BaseExhibitorValidator<T> : AbstractValidator<T>
    {
        protected void AddCommonRules(Func<T, UpsertExhibitorDto> dtoSelector)
        {
            List<string> supportedCOdes = Country.GetCountries()
                .Select(x => x.Alpha2.ToString()).ToList();

            RuleFor(v => dtoSelector(v).Country).NotEmpty().MaximumLength(2).Must(x => supportedCOdes.Contains(x));
            RuleFor(v => dtoSelector(v).Organization).NotEmpty().MaximumLength(200);
            RuleFor(v => dtoSelector(v).MemberNumber).NotEmpty().MaximumLength(200);
            RuleFor(v => dtoSelector(v).Country).NotEmpty().MaximumLength(200);
            RuleFor(v => dtoSelector(v).City).NotEmpty().MaximumLength(200);
            RuleFor(v => dtoSelector(v).Street).NotEmpty().MaximumLength(200);
            RuleFor(v => dtoSelector(v).HouseNumber).NotEmpty().MaximumLength(200);
            RuleFor(v => dtoSelector(v).ZipCode).NotEmpty().MaximumLength(200);
        }
    }
}
