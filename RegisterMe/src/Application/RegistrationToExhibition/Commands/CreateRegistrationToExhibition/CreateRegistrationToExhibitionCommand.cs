#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Commands.CreateRegistrationToExhibition;

// ReSharper disable always UnusedType.Global
public record CreateRegistrationToExhibitionCommand : IRequest<Result<int>>
{
    public required CreateRegistrationToExhibitionDto RegistrationToExhibition { get; init; } = null!;
}

public class CreateRegistrationToExhibitionCommandValidator : AbstractValidator<CreateRegistrationToExhibitionCommand>
{
    private readonly IApplicationDbContext _applicationDbContext;

    public CreateRegistrationToExhibitionCommandValidator(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
        RuleFor(x => x.RegistrationToExhibition.ExhibitionId).ForeignKeyValidator();
        RuleFor(x => x.RegistrationToExhibition.ExhibitorId).ForeignKeyValidator();
        RuleFor(x => x.RegistrationToExhibition.AdvertisementId).ForeignKeyValidator();

        RuleFor(x => x)
            .Must(DatabaseValidation)
            .WithMessage("Invalid data");
    }

    private bool DatabaseValidation(CreateRegistrationToExhibitionCommand createRegistrationToExhibitionCommand)
    {
        if (!_applicationDbContext.Exhibitions.Any(x =>
                x.Id == createRegistrationToExhibitionCommand.RegistrationToExhibition.ExhibitionId))
        {
            return false;
        }

        return _applicationDbContext.Exhibitors.Any(x =>
            x.Id == createRegistrationToExhibitionCommand.RegistrationToExhibition.ExhibitorId);
    }
}

public class CreateRegistrationToExhibitionCommandHandler(
    IUser user,
    IAuthorizationService authorizationService,
    IRegistrationToExhibitionService registrationToExhibitionService)
    : IRequestHandler<CreateRegistrationToExhibitionCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateRegistrationToExhibitionCommand request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult =
            await authorizationService.AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitorId(request.RegistrationToExhibition.ExhibitorId),
                Operations.Read);
        Guard.Against.UnAuthorized(authorizationResult);
        Result<int> result = await registrationToExhibitionService.CreateRegistrationToExhibition(
            request.RegistrationToExhibition,
            cancellationToken);
        return result;
    }
}
