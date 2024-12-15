#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Validators;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Exhibitions.Commands.UpdatePriceGroup;

// ReSharper disable always UnusedType.Global
public record UpdatePriceGroupCommand : IRequest<Result<string>>
{
    public required List<string> GroupsIds { get; init; }
    public required List<PriceDays> PriceDays { get; init; }
    public required string OriginalPricesId { get; init; }
}

public class UpdatePriceGroupCommandValidator : BasePricesValidator.BasePriceCommandValidator<UpdatePriceGroupCommand>
{
    public UpdatePriceGroupCommandValidator()
    {
        AddCommonRules(x => new BasePriceValidatedDto { GroupsIds = x.GroupsIds, PriceDays = x.PriceDays });

        RuleFor(x => x.OriginalPricesId)
            .NotNull()
            .Must(HasValidPrices)
            .WithMessage("OriginalPricesId must be a non-empty, comma-separated list of distinct positive integers.");
    }

    private bool HasValidPrices(string originalPricesId)
    {
        if (string.IsNullOrWhiteSpace(originalPricesId))
        {
            return false;
        }

        string[] ids = originalPricesId.Split(',');
        if (ids.Length < 1)
        {
            return false;
        }

        string[] distinctIds = ids.Distinct().ToArray();
        return distinctIds.Length == ids.Length && ids.All(id => int.TryParse(id, out int result) && result > 0);
    }
}

public class UpdatePriceGroupCommandHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<UpdatePriceGroupCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdatePriceGroupCommand request, CancellationToken cancellationToken)
    {
        Result<BigPriceDto> prices =
            await exhibitionService.GetPricesGroupsById(request.OriginalPricesId, cancellationToken);
        AuthorizationResult authorizationResult =
            await authorizationService.AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitionId(prices.Value.ExhibitionId),
                Operations.Update);
        Guard.Against.UnAuthorized(authorizationResult);
        string result = await exhibitionService.UpdatePriceGroup(request.OriginalPricesId, request.GroupsIds,
            request.PriceDays, cancellationToken);
        return result;
    }
}
