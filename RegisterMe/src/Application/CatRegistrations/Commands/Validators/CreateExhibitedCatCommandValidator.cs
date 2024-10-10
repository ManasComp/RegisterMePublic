#region

using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Services.Ems;

#endregion

namespace RegisterMe.Application.CatRegistrations.Commands.Validators;

public class CreateExhibitedCatCommandValidator : AbstractValidator<CreateExhibitedCatDto>
{
    public CreateExhibitedCatCommandValidator(TimeProvider dateTimeProvider)
    {
        RuleFor(x => x.TitleBeforeName).Length(1, 75).When(x => !string.IsNullOrEmpty(x.TitleBeforeName));
        RuleFor(x => x.TitleAfterName).Length(1, 75).When(x => !string.IsNullOrEmpty(x.TitleAfterName));
        RuleFor(x => x.Name).NotEmpty().Length(1, 75);
        RuleFor(x => x.Ems).NotEmpty().Length(1, 75);

        RuleFor(x => x.PedigreeNumber).NotEmpty().When(x =>
            x.BirthDate.AddMonths(6) <=
            DateOnly.FromDateTime(dateTimeProvider.GetLocalNow()
                .DateTime) &&
            x is
            {
                Mother: not null, Father: not null
            }); // cats younger thar 6 moths or home cats do not need pedigree number
        RuleFor(x => x.PedigreeNumber).Length(1, 75).When(x => !string.IsNullOrEmpty(x.PedigreeNumber));

        RuleFor(x => x.Colour).NotEmpty().NotEmpty().When(x =>
            !x.Ems.Contains("HCS") && !x.Ems.Contains("HCL"));
        RuleFor(x => x.Colour).NotEmpty().Length(1, 75).When(x => !string.IsNullOrEmpty(x.Colour));

        RuleFor(x => x.BirthDate).NotEmpty()
            .LessThan(DateOnly.FromDateTime(dateTimeProvider.GetLocalNow().DateTime)) // cat has to be already born
            .GreaterThan(DateOnly.FromDateTime(DateTime.Now.AddYears(-40)));

        RuleFor(x => x.Breed).NotEmpty().Length(1, 75);
        RuleFor(x => x.Group).NotEmpty().InclusiveBetween(1, 12)
            .When(x => EmsCode.RequiresGroup(x.Ems));
        RuleFor(x => x.Group).Empty()
            .When(x => !EmsCode.RequiresGroup(x.Ems));
        RuleFor(x => x.Sex).NotEmpty().IsInEnum();
        RuleFor(x => x.Breeder!).SetValidator(new CreateBreederCommandValidator()).When(x => x.Breeder != null);
        RuleFor(x => x.Father!).SetValidator(new CreateParentCommandValidator()).When(x => x.Father != null);
        RuleFor(x => x.Mother!).SetValidator(new CreateParentCommandValidator()).When(x => x.Mother != null);
    }
}
