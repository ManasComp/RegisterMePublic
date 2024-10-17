#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;

#endregion

namespace RegisterMe.Application.Exhibitions.Queries.GetExhibitionById;

// ReSharper disable always UnusedType.Global
public record GetExhibitionByIdQuery : IRequest<BriefExhibitionDto>
{
    public required int ExhibitionId { get; init; }
}

public class GetExhibitionByIdQueryValidator : AbstractValidator<GetExhibitionByIdQuery>
{
    public GetExhibitionByIdQueryValidator()
    {
        RuleFor(x => x.ExhibitionId).ForeignKeyValidator();
    }
}

public class GetExhibitionByIdQueryHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<GetExhibitionByIdQuery, BriefExhibitionDto>
{
    public async Task<BriefExhibitionDto> Handle(GetExhibitionByIdQuery request, CancellationToken cancellationToken)
    {
        BriefExhibitionDto exhibition =
            await exhibitionService.GetExhibitionById(request.ExhibitionId, cancellationToken);

        if (!exhibition.IsPublished)
        {
            AuthorizationResult result = await authorizationService.AuthorizeAsync(
                AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitionId(request.ExhibitionId), Operations.Read);
            Guard.Against.UnAuthorized(result);
        }

        return exhibition;
    }
}
