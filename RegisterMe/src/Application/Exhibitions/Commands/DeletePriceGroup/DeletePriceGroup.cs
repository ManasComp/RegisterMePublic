#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Exhibitions.Commands.DeletePriceGroup;

// ReSharper disable always UnusedType.Global
public record DeletePriceGroupCommand : IRequest<Result>
{
    public required string PriceIds { get; init; } = null!;
}

public class DeletePriceGroupCommandValidator : AbstractValidator<DeletePriceGroupCommand>
{
    public DeletePriceGroupCommandValidator()
    {
        RuleFor(x => x.PriceIds)
            .NotNull().WithMessage("PriceIds cannot be null.")
            .NotEmpty().WithMessage("PriceIds cannot be empty.")
            .Must(x => x.Split(',').Length > 0).WithMessage("PriceIds must contain at least one id.")
            .Must(x => x.Split(',').All(id => int.TryParse(id, out int result) && result > 0))
            .WithMessage("All PriceIds must be valid integers greater than 0.")
            .Must(x => x.Split(',').Distinct().Count() == x.Split(',').Length)
            .WithMessage("PriceIds must be unique.");
    }
}

public class DeletePriceGroupCommandHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<DeletePriceGroupCommand, Result>
{
    public async Task<Result> Handle(DeletePriceGroupCommand request, CancellationToken cancellationToken)
    {
        Result<BigPriceDto> advertisement =
            await exhibitionService.GetPricesGroupsById(request.PriceIds, cancellationToken);
        Guard.Against.ResultFailed(advertisement);
        AuthorizationResult authResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(advertisement.Value.ExhibitionId), Operations.Update);
        Guard.Against.UnAuthorized(authResult);
        Result result = await exhibitionService.DeletePriceGroup(request.PriceIds, cancellationToken);
        return result;
    }
}
