#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Validators;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Exhibitions.Commands.CreateAdvertisement;

// ReSharper disable always UnusedType.Global
public record CreateAdvertisementCommand : IRequest<Result<int>>
{
    public required UpsertAdvertisementDto Advertisement { get; init; }
    public required int ExhibitionId { get; init; }
}

public class
    CreateAdvertisementCommandValidator : BaseAdvertisement.BaseAdvertisementCommandValidator<
    CreateAdvertisementCommand>
{
    public CreateAdvertisementCommandValidator()
    {
        AddCommonRules(x => x.Advertisement);
        RuleFor(x => x.ExhibitionId).ForeignKeyValidator();
    }
}

public class CreateAdvertisementCommandHandler(
    IExhibitionService exhibitionService,
    IUser user,
    IAuthorizationService authorizationService)
    : IRequestHandler<CreateAdvertisementCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateAdvertisementCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(request.ExhibitionId), Operations.Update);
        Guard.Against.UnAuthorized(authResult);
        Result<int> data = await exhibitionService.CreateAdvertisement(request.Advertisement, request.ExhibitionId,
            cancellationToken);
        return data;
    }
}
