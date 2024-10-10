#region

using RegisterMe.Application.CatRegistrations.Dtos;

#endregion

namespace RegisterMe.Application.CatRegistrations.Commands.Validators;

public class CreateParentCommandValidator : AbstractValidator<ParentDto>
{
    public CreateParentCommandValidator()
    {
        RuleFor(x => x.TitleBeforeName).Length(1, 75).When(x => !string.IsNullOrEmpty(x.TitleBeforeName));
        RuleFor(x => x.TitleAfterName).Length(1, 75).When(x => !string.IsNullOrEmpty(x.TitleAfterName));
        RuleFor(x => x.Name).NotEmpty().Length(1, 75);
        RuleFor(x => x.Ems).NotEmpty().Length(1, 75);
        RuleFor(x => x.PedigreeNumber).NotEmpty().Length(1, 75);
        RuleFor(x => x.Colour).NotEmpty().When(x =>
            !x.Ems.Contains("HCS") && !x.Ems.Contains("HCL"));
        RuleFor(x => x.Colour).Length(1, 75).When(x => !string.IsNullOrEmpty(x.Colour));
    }
}
