#region

using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Common.Validators;

#endregion

namespace RegisterMe.Application.CatRegistrations.Commands.Validators;

public class CreateCatDayCommandValidator : AbstractValidator<CreateCatDayDto>
{
    public CreateCatDayCommandValidator()
    {
        RuleFor(address => address.ExhibitionDayId)
            .ForeignKeyValidator();
        RuleFor(address => address.ExhibitorsCage)
            .Must(x => x > 0)
            .When(x => x.ExhibitorsCage.HasValue);
        RuleFor(x => x.GroupsIds).NotEmpty();
        RuleFor(x => x.GroupsIds.Count).GreaterThan(0);
        RuleForEach(x => x.GroupsIds).ForeignKeyValidator();
    }
}
