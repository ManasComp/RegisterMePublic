#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Cages;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exceptions;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Exhibitions.Commands.DeleteRentedCages;

// ReSharper disable always UnusedType.Global
public record DeleteRentedCagesCommand : IRequest<Result>
{
    public required string CagesId { get; init; }
}

public class DeleteRentedCagesCommandValidator : AbstractValidator<DeleteRentedCagesCommand>
{
    public DeleteRentedCagesCommandValidator()
    {
        RuleFor(x => x.CagesId).NotEmpty().NotEmpty();
        RuleFor(x => x.CagesId)
            .Must(x => x.Split(",").All(item => int.TryParse(item, out _)))
            .WithMessage("CagesId must be a comma-separated list of integers.");
    }
}

public class DeleteRentedCagesCommandHandler(
    ICagesService cagesService,
    IUser user,
    IAuthorizationService authorizationService,
    IApplicationDbContext applicationDbContext) : IRequestHandler<DeleteRentedCagesCommand, Result>
{
    public async Task<Result> Handle(DeleteRentedCagesCommand request, CancellationToken cancellationToken)
    {
        List<int> cageIds = CagesService.FromGroupIdToIds(request.CagesId);
        int exhibitionId = await applicationDbContext.RentedCages
            .Where(x => cageIds.Contains(x.Id))
            .Select(x => x.ExhibitionDays.First().ExhibitionId)
            .FirstOrDefaultAsync(cancellationToken);

        AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(exhibitionId), Operations.Update);
        Guard.Against.UnAuthorized(authorizationResult);
        Result result = await cagesService.DeleteRentedCages(request.CagesId, cancellationToken);
        return result;
    }
}
