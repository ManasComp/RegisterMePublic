#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.CatRegistrations.Commands.DeleteCatRegistration;

// ReSharper disable always UnusedType.Global
public record DeleteCatRegistrationCommand : IRequest<Result>
{
    public required int CatRegistrationId { get; init; }
}

public class DeleteCatRegistrationCommandValidator : AbstractValidator<DeleteCatRegistrationCommand>
{
    public DeleteCatRegistrationCommandValidator()
    {
        RuleFor(x => x.CatRegistrationId).ForeignKeyValidator();
    }
}

public class DeleteCatRegistrationCommandHandler(
    ICatRegistrationService catRegistrationService,
    IApplicationDbContext context,
    IUser user,
    IAuthorizationService authorizationService) : IRequestHandler<DeleteCatRegistrationCommand, Result>
{
    public async Task<Result> Handle(DeleteCatRegistrationCommand request, CancellationToken cancellationToken)
    {
        int registrationToExhibitionId = AuthorizationHelperMethods.GetRegistrationToExhibitionIdFromCatRegistrationId(
            context,
            request.CatRegistrationId);

        AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId), Operations.Update);
        Guard.Against.UnAuthorized(authorizationResult);

        bool isOrganizationAdmin = await context.CatRegistrations
            .Where(x => x.Id == request.CatRegistrationId)
            .AnyAsync(x => x.RegistrationToExhibition.Exhibition.Organization.Administrator.Any(y => y.Id == user.Id),
                cancellationToken);
        Result result = await catRegistrationService.DeleteCatRegistration(request.CatRegistrationId,
            isOrganizationAdmin,
            cancellationToken);

        return result;
    }
}
