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

namespace RegisterMe.Application.Exhibitions.Commands.UpdateExhibition;

// ReSharper disable always UnusedType.Global
public record UpdateExhibitionCommand : IRequest<Result>
{
    public required UpdateExhibitionDto UpdateExhibitionDto { get; init; } = null!;
}

public class
    UpdateExhibitionCommandValidator : BaseExhibitionValidator.BaseExhibitionCommandValidator<UpdateExhibitionCommand>
{
    public UpdateExhibitionCommandValidator()
    {
        AddCommonRules(x => x.UpdateExhibitionDto);
        RuleFor(x => x.UpdateExhibitionDto.Id).ForeignKeyValidator();
    }
}

public class UpdateExhibitionCommandHandler(
    IUser user,
    IAuthorizationService authorizationService,
    IExhibitionService exhibitionService) : IRequestHandler<UpdateExhibitionCommand, Result>
{
    public async Task<Result> Handle(UpdateExhibitionCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authResult = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(request.UpdateExhibitionDto.Id), Operations.Update);
        Guard.Against.UnAuthorized(authResult);
        Result result = await exhibitionService.UpdateExhibition(request.UpdateExhibitionDto, cancellationToken);
        return result;
    }
}
