#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Organizations.Dtos;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Organizations.Commands.CreateOrganization;

// ReSharper disable always UnusedType.Global
public record CreateOrganizationCommand : IRequest<Result<int>>
{
    public required CreateOrganizationDto CreateOrganizationDto { get; init; } = null!;
}

public class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationCommandValidator()
    {
        RuleFor(v => v.CreateOrganizationDto.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.CreateOrganizationDto.Address)
            .NotEmpty().WithMessage("Address is required.")
            .Length(2, 100).WithMessage("Address must be between 2 and 100 characters.");

        RuleFor(x => x.CreateOrganizationDto.AdminId)
            .NotEmpty().WithMessage("AdminId is required.")
            .Length(2, 100).WithMessage("AdminId must be between 2 and 100 characters.");

        RuleFor(x => x.CreateOrganizationDto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not a valid email address.");

        RuleFor(x => x.CreateOrganizationDto.Ico)
            .NotEmpty().WithMessage("ICO is required.");

        RuleFor(x => x.CreateOrganizationDto.TelNumber)
            .NotEmpty().WithMessage("Telephone number is required.");

        RuleFor(x => x.CreateOrganizationDto.Website)
            .NotEmpty().WithMessage("Website is required.")
            .Must(website => Uri.TryCreate(website, UriKind.RelativeOrAbsolute, out _))
            .WithMessage("Website is not a valid URL.");
    }
}

public class CreateOrganizationCommandHandler(
    IOrganizationService organizationService,
    IUser user,
    IAuthorizationService authorizationService)
    : IRequestHandler<CreateOrganizationCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult =
            await authorizationService.AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeOwnDataId(request.CreateOrganizationDto.AdminId),
                Operations.Create);
        Guard.Against.UnAuthorized(authorizationResult);

        Result<int> result =
            await organizationService.CreateOrganization(request.CreateOrganizationDto, cancellationToken);
        return result;
    }
}
