#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.CatRegistrations.Commands.Validators;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exceptions;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.CatRegistrations.Commands.UpdateCatRegistrationCommand;

// ReSharper disable always UnusedType.Global
public record UpdateCatRegistrationCommand : IRequest<Result<int>>
{
    public required UpdateCatRegistrationDto CatRegistration { get; init; } = null!;
}

public class UpdateCatRegistrationCommandValidator : AbstractValidator<UpdateCatRegistrationCommand>
{
    public UpdateCatRegistrationCommandValidator(TimeProvider dateTimeProvider)
    {
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

public class UpdateCatRegistrationCommandHandler(
    ICatRegistrationService catRegistrationService,
    IAuthorizationService authorizationService,
    IApplicationDbContext applicationDbContext,
    IUser user)
    : IRequestHandler<UpdateCatRegistrationCommand, Result<int>>
{
    public async Task<Result<int>> Handle(UpdateCatRegistrationCommand request, CancellationToken cancellationToken)
    {
        int registrationToExhibitionId = AuthorizationHelperMethods.GetRegistrationToExhibitionIdFromCatRegistrationId(
            applicationDbContext,
            request.CatRegistration.Id);
        AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId), Operations.Update);
        Guard.Against.UnAuthorized(authorizationResult);

        bool isOrganizationAdmin = await applicationDbContext.CatRegistrations
            .Where(x => x.Id == request.CatRegistration.Id)
            .AnyAsync(x => x.RegistrationToExhibition.Exhibition.Organization.Administrator.Any(y => y.Id == user.Id),
                cancellationToken);

        Result<int> result =
            await catRegistrationService.UpdateCatRegistration(request.CatRegistration,
                isOrganizationAdmin, cancellationToken);

        return result;
    }
}
