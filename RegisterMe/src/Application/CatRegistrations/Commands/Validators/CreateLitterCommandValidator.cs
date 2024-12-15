#region

using RegisterMe.Application.CatRegistrations.Dtos;

#endregion

namespace RegisterMe.Application.CatRegistrations.Commands.Validators;

public class CreateLitterCommandValidator : AbstractValidator<CreateLitterDto>
{
    public CreateLitterCommandValidator()
    {
        RuleFor(x => x.PassOfOrigin).MaximumLength(75);
        RuleFor(x => x.NameOfBreedingStation).NotEmpty().Length(1, 75);
        RuleFor(x => x.Breed).NotEmpty().Length(1, 75);
        RuleFor(x => x.Breeder).NotEmpty();
        RuleFor(x => x.Breeder).SetValidator(new CreateBreederCommandValidator());
        RuleFor(x => x.Father).NotEmpty();
        RuleFor(x => x.Father).SetValidator(new CreateParentCommandValidator());
        RuleFor(x => x.Mother).NotEmpty();
        RuleFor(x => x.Mother).SetValidator(new CreateParentCommandValidator());
    }
}
