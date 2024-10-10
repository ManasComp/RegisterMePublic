#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;

#endregion

namespace RegisterMe.Application.Cages.Queries.RecordWithGivenExhibitionDayIdAndCatRegistrationIdExistWithCageId;

// ReSharper disable always UnusedType.Global
public record RecordWithGivenExhibitionDayIdAndCatRegistrationIdExistWithCageIdQuery : IRequest<bool>
{
    public required int CageId { get; init; }
    public required int CatRegistrationId { get; init; }
    public required int ExhibitionDayId { get; init; }
}

public class RecordWithGivenExhibitionDayIdAndCatRegistrationIdExistWithCageIdQueryValidator : AbstractValidator<
    RecordWithGivenExhibitionDayIdAndCatRegistrationIdExistWithCageIdQuery>
{
    public RecordWithGivenExhibitionDayIdAndCatRegistrationIdExistWithCageIdQueryValidator()
    {
        RuleFor(x => x.CageId).ForeignKeyValidator();
        RuleFor(x => x.CatRegistrationId).ForeignKeyValidator();
        RuleFor(x => x.ExhibitionDayId).ForeignKeyValidator();
    }
}

public class RecordWithGivenExhibitionDayIdAndCatRegistrationIdExistWithCageIdQueryHandler(
    ICagesService cagesService,
    IAuthorizationService authorizationService,
    IApplicationDbContext applicationDbContext,
    IUser user)
    : IRequestHandler<RecordWithGivenExhibitionDayIdAndCatRegistrationIdExistWithCageIdQuery, bool>
{
    public async Task<bool> Handle(RecordWithGivenExhibitionDayIdAndCatRegistrationIdExistWithCageIdQuery request,
        CancellationToken cancellationToken)
    {
        int registrationToExhibitionId = AuthorizationHelperMethods.GetRegistrationToExhibitionIdFromCatRegistrationId(
            applicationDbContext,
            request.CatRegistrationId);
        AuthorizationResult authResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(authResult);
        // as this only returns bool, do not check other authorizations

        bool result =
            await cagesService.RecordWithGivenExhibitionDayIdAndCatRegistrationIdExistWithCageId(request.CageId,
                request.CatRegistrationId, request.ExhibitionDayId, cancellationToken);
        return result;
    }
}
