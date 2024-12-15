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

namespace RegisterMe.Application.Exhibitions.Commands.CreateExhibition;

// ReSharper disable always UnusedType.Global
public record CreateExhibitionCommand : IRequest<Result<int>>
{
    public CreateExhibitionDto CreateExhibitionDto { get; init; } = null!;
}

public class
    CreateExhibitionCommandValidator : BaseExhibitionValidator.BaseExhibitionCommandValidator<CreateExhibitionCommand>
{
    public CreateExhibitionCommandValidator()
    {
        AddCommonRules(x => x.CreateExhibitionDto);
        RuleFor(x => x.CreateExhibitionDto.OrganizationId).ForeignKeyValidator();
    }
}

public class CreateExhibitionCommandHandler(
    IExhibitionService exhibitionService,
    IAuthorizationService authorizationService,
    IUser user)
    : IRequestHandler<CreateExhibitionCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateExhibitionCommand request, CancellationToken cancellationToken)
    {
        int organizationId = request.CreateExhibitionDto.OrganizationId;
        AuthorizationResult authResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeOrganizationId(organizationId), Operations.Update);
        Guard.Against.UnAuthorized(authResult);

        Result<int> result = await exhibitionService.CreateExhibition(request.CreateExhibitionDto, cancellationToken);
        return result;
    }
}
