#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Services.Groups;

#endregion

namespace RegisterMe.Application.Exhibitions.Queries.GetAllExhibitionGroupsThatAreNotFullyRegistered;

// ReSharper disable always UnusedType.Global
public record GetAllExhibitionGroupsThatAreNotFullyRegisteredQuery : IRequest<List<DatabaseGroupDto>>
{
    public required int ExhibitionId { get; init; }
}

public class
    GetAllExhibitionGroupsThatAreNotFullyRegisteredQueryValidator : AbstractValidator<
    GetAllExhibitionGroupsThatAreNotFullyRegisteredQuery>
{
    public GetAllExhibitionGroupsThatAreNotFullyRegisteredQueryValidator()
    {
        RuleFor(x => x.ExhibitionId).ForeignKeyValidator();
    }
}

public class GetAllExhibitionGroupsThatAreNotFullyRegisteredQueryHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<GetAllExhibitionGroupsThatAreNotFullyRegisteredQuery, List<DatabaseGroupDto>>
{
    public async Task<List<DatabaseGroupDto>> Handle(GetAllExhibitionGroupsThatAreNotFullyRegisteredQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(request.ExhibitionId), Operations.Read);
        Guard.Against.UnAuthorized(result);

        List<DatabaseGroupDto> data =
            await exhibitionService.GetExhibitionGroupsThatAreNotFullyRegistered(request.ExhibitionId);
        return data;
    }
}
