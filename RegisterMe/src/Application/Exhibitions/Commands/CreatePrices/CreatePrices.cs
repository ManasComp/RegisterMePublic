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

namespace RegisterMe.Application.Exhibitions.Commands.CreatePrices;

// ReSharper disable always UnusedType.Global
public record CreatePriceGroupCommand : IRequest<Result<string>>
{
    public required List<string> GroupsIds { get; init; }
    public required int ExhibitionId { get; init; }
    public required List<PriceDays> PriceDays { get; init; }
}

public class CreatePriceGroupCommandValidator : BasePricesValidator.BasePriceCommandValidator<CreatePriceGroupCommand>
{
    public CreatePriceGroupCommandValidator()
    {
        AddCommonRules(x => new BasePriceValidatedDto { GroupsIds = x.GroupsIds, PriceDays = x.PriceDays });
        RuleFor(x => x.ExhibitionId).ForeignKeyValidator();
    }
}

public class CreatePriceGroupCommandHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<CreatePriceGroupCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreatePriceGroupCommand request, CancellationToken cancellationToken)
    {
        int exhibitionId = request.ExhibitionId;
        AuthorizationResult authResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(exhibitionId), Operations.Update);
        Guard.Against.UnAuthorized(authResult);
        return await exhibitionService.CreatePrices(request.GroupsIds, request.ExhibitionId, request.PriceDays,
            cancellationToken);
    }
}
