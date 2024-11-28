#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.CatRegistrations.Commands.Validators;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.CatRegistrations.Commands.CreateCatRegistration;

// ReSharper disable always UnusedType.Global
public record CreateCatRegistrationCommand : IRequest<Result<int>>
{
    public required CreateCatRegistrationDto CatRegistration { get; init; } = null!;
}

public class CreateCatRegistrationCommandValidator : AbstractValidator<CreateCatRegistrationCommand>
{
    public CreateCatRegistrationCommandValidator(TimeProvider dateTimeProvider)
    {
        RuleFor(x => x.CatRegistration.RegistrationToExhibitionId)
            .ForeignKeyValidator();

        RuleFor(x => x.CatRegistration.Note)
            .Length(1, 75)
            .When(s => !string.IsNullOrEmpty(s.CatRegistration.Note));

        RuleFor(x => x.CatRegistration.CatDays).NotEmpty();
        RuleFor(x => x.CatRegistration.CatDays.Count).GreaterThan(0);
        RuleForEach(x => x.CatRegistration.CatDays).SetValidator(new CreateCatDayCommandValidator());
        RuleFor(x => x.CatRegistration.Litter!).SetValidator(new CreateLitterCommandValidator())
            .When(x => x.CatRegistration.Litter != null);
        RuleFor(x => x.CatRegistration.ExhibitedCat!)
            .SetValidator(new CreateExhibitedCatCommandValidator(dateTimeProvider))
            .When(x => x.CatRegistration.ExhibitedCat != null);
    }
}

public class CreateCatRegistrationCommandHandler(
    ICatRegistrationService catRegistrationService,
    IAuthorizationService authorizationService,
    IApplicationDbContext applicationDbContext,
    IUser user)
    : IRequestHandler<CreateCatRegistrationCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateCatRegistrationCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeRegistrationToExhibitionId(request.CatRegistration.RegistrationToExhibitionId),
            Operations.Update);
        Guard.Against.UnAuthorized(authorizationResult);

        bool isOrganizationAdmin = await applicationDbContext.RegistrationsToExhibition
            .Where(x => x.Id == request.CatRegistration.RegistrationToExhibitionId)
            .AnyAsync(x => x.Exhibition.Organization.Administrator.Any(y => y.Id == user.Id), cancellationToken);

        Result<int> result =
            await catRegistrationService.CreateCatRegistration(request.CatRegistration, isOrganizationAdmin,
                cancellationToken);
        return result;
    }
}
