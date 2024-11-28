#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Cages.Dtos.Combination;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;

#endregion

namespace RegisterMe.Application.Cages.Queries.GetAvailableRentedCageTypes;

// ReSharper disable always UnusedType.Global
public record GetAvailableRentedCageTypesQuery : IRequest<CagesPerDayDto>
{
    public required int ExhibitionDayId { get; init; }
    public required int RegistrationToExhibitionId { get; init; }
    public required bool IsLitter { get; init; }
    public required int? CatRegistrationId { get; init; }
}

public class GetAvailableRentedCageTypesQueryValidator : AbstractValidator<GetAvailableRentedCageTypesQuery>
{
    public GetAvailableRentedCageTypesQueryValidator()
    {
        RuleFor(x => x.ExhibitionDayId).ForeignKeyValidator();
        RuleFor(x => x.RegistrationToExhibitionId).ForeignKeyValidator();
        RuleFor(x => x.CatRegistrationId).OptionalForeignKeyValidator();
    }
}

public class
    GetAvailableRentedCageTypesQueryHandler(
        ICagesService cagesService,
        IAuthorizationService authorizationService,
        IApplicationDbContext applicationDbContext,
        IUser user) : IRequestHandler<GetAvailableRentedCageTypesQuery, CagesPerDayDto>
{
    public async Task<CagesPerDayDto> Handle(GetAvailableRentedCageTypesQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeRegistrationToExhibitionId(request.RegistrationToExhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(result);

        int exhibitionId = await applicationDbContext.ExhibitionDays
            .Where(x => x.Id == request.ExhibitionDayId)
            .Select(x => x.ExhibitionId)
            .FirstOrDefaultAsync(cancellationToken);

        AuthorizationResult result1 = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(exhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(result1);

        if (request.CatRegistrationId.HasValue)
        {
            bool contains = await applicationDbContext.RegistrationsToExhibition
                .Where(x => x.Id == request.RegistrationToExhibitionId)
                .SelectMany(x => x.CatRegistrations)
                .Select(x => x.Id)
                .ContainsAsync(request.CatRegistrationId.Value, cancellationToken);
            if (!contains)
            {
                throw new ForbiddenAccessException("You are not allowed to access this cat registration");
            }
        }

        CagesPerDayDto dto = await cagesService.GetAvailableCageGroupTypesAndOwnCages(request.ExhibitionDayId,
            request.RegistrationToExhibitionId, request.IsLitter, request.CatRegistrationId, cancellationToken);
        return dto;
    }
}
