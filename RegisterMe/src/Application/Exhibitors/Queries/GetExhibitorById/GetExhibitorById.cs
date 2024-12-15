#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitors.Dtos;

#endregion

namespace RegisterMe.Application.Exhibitors.Queries.GetExhibitorById;

// ReSharper disable always UnusedType.Global
public record GetExhibitorByIdQuery : IRequest<ExhibitorAndUserDto>
{
    public required int ExhibitorId { get; init; }
}

public class GetExhibitorByIdQueryValidator : AbstractValidator<GetExhibitorByIdQuery>
{
    public GetExhibitorByIdQueryValidator()
    {
        RuleFor(x => x.ExhibitorId).ForeignKeyValidator();
    }
}

public class GetExhibitorByIdQueryHandler(
    IExhibitorService exhibitorService,
    IUser user,
    IAuthorizationService authorizationService) : IRequestHandler<GetExhibitorByIdQuery, ExhibitorAndUserDto>
{
    public async Task<ExhibitorAndUserDto> Handle(GetExhibitorByIdQuery request, CancellationToken cancellationToken)
    {
        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitorId(request.ExhibitorId), Operations.Read);
        Guard.Against.UnAuthorized(result);

        return await exhibitorService.GetExhibitorById(request.ExhibitorId, cancellationToken);
    }
}
