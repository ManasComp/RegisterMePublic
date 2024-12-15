#region

using RegisterMe.Application.Exhibitions.Dtos;

#endregion

namespace RegisterMe.Application.Exhibitions.Validators;

public class BaseExhibitionValidator
{
    public abstract class BaseExhibitionCommandValidator<T> : AbstractValidator<T>
    {
        protected void AddCommonRules(Func<T, BaseExhibitionValidatedDto> dtoSelector)
        {
            RuleFor(x => dtoSelector(x).Name).NotEmpty().MaximumLength(60);
            RuleFor(x => dtoSelector(x).Url).NotEmpty().MaximumLength(60);
            RuleFor(x => dtoSelector(x).Url).Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrEmpty(dtoSelector(x).Url));
            RuleFor(x => dtoSelector(x).Description).NotEmpty().MaximumLength(500);
            RuleFor(x => dtoSelector(x).BankAccount).NotEmpty().MaximumLength(60);
            RuleFor(x => dtoSelector(x).Iban).NotEmpty().Length(24);
            RuleFor(x => dtoSelector(x).Phone).NotEmpty().MaximumLength(40);
            RuleFor(x => dtoSelector(x).Email).EmailAddress();
            RuleFor(x => dtoSelector(x).RegistrationStart).NotEmpty();
            RuleFor(x => dtoSelector(x).RegistrationEnd).NotEmpty();
            RuleFor(x => dtoSelector(x).ExhibitionStart).NotEmpty();
            RuleFor(x => dtoSelector(x).ExhibitionEnd).NotEmpty();
            RuleFor(x => dtoSelector(x).DeleteNotFinishedRegistrationsAfterHours).GreaterThan(1).LessThanOrEqualTo(48);
            RuleFor(x => x).Custom((x, context) =>
            {
                BaseExhibitionValidatedDto dto = dtoSelector(x);
                if (dto.RegistrationStart >= dto.RegistrationEnd)
                {
                    context.AddFailure("RegistrationStart",
                        " Datum startu registrace výstavy musí být před datem konce výstavy.");
                }

                if (dto.ExhibitionStart > dto.ExhibitionEnd)
                {
                    context.AddFailure("ExhibitionStart",
                        "Datum zahájení výstavy musí být před datem ukončení výstavy nebo se musí jednat o stejný den.");
                }

                if (dto.RegistrationEnd >= dto.ExhibitionStart)
                {
                    context.AddFailure("RegistrationStart",
                        "Datum zahájení registrace musí být před datem zahájení výstavy.");
                }
            });
        }
    }
}
