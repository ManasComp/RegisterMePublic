#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.Exhibitors.Validators;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Exhibitors.Commands.CreateExhibitor;

// ReSharper disable always UnusedType.Global
public record CreateExhibitorCommand : IRequest<Result<int>>
{
    public required string UserId { get; init; } = null!;
    public required UpsertExhibitorDto Exhibitor { get; init; } = null!;
}

public class CreateExhibitorCommandValidator : BaseExhibitor.BaseExhibitorValidator<CreateExhibitorCommand>
{
    public CreateExhibitorCommandValidator(UserManager<ApplicationUser> userManager)
    {
        AddCommonRules(v => v.Exhibitor);

        RuleFor(v => v.UserId)
            .MustAsync(async (userId, _) =>
            {
                ApplicationUser? user = await userManager.FindByIdAsync(userId);
                return user != null;
            }).WithMessage("User does not exist");
    }
}

public class CreateExhibitorCommandHandler(
    IExhibitorService exhibitorService,
    IUser user,
    IAuthorizationService authorizationService)
    : IRequestHandler<CreateExhibitorCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateExhibitorCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult =
            await authorizationService.AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeOwnDataId(request.UserId),
                Operations.Create);
        Guard.Against.UnAuthorized(authorizationResult);

        UpsertExhibitorDto exhibitor = new()
        {
            Organization = request.Exhibitor.Organization,
            MemberNumber = request.Exhibitor.MemberNumber,
            Country = request.Exhibitor.Country,
            City = request.Exhibitor.City,
            Street = request.Exhibitor.Street,
            HouseNumber = request.Exhibitor.HouseNumber,
            ZipCode = request.Exhibitor.ZipCode,
            IsPartOfCsch = request.Exhibitor.IsPartOfCsch,
            EmailToOrganization = request.Exhibitor.EmailToOrganization,
            IsPartOfFife = request.Exhibitor.IsPartOfFife
        };
        return await exhibitorService.CreateExhibitor(exhibitor, request.UserId, cancellationToken);
    }
}
