#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Organizations.Dtos;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Organizations.Commands.UpdateOrganization;

// ReSharper disable always UnusedType.Global
public record UpdateOrganizationCommand : IRequest<Result>
{
    public required UpdateOrganizationDto OrganizationDto { get; init; } = null!;
}

public class UpdateOrganizationCommandValidator : AbstractValidator<UpdateOrganizationCommand>
{
    public UpdateOrganizationCommandValidator()
    {
        RuleFor(v => v.OrganizationDto.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.OrganizationDto.Address)
            .NotEmpty().WithMessage("Address is required.")
            .Length(2, 100).WithMessage("Address must be between 2 and 100 characters.");

        RuleFor(x => x.OrganizationDto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not a valid email address.");

        RuleFor(x => x.OrganizationDto.TelNumber)
            .NotEmpty().WithMessage("Telephone number is required.");

        RuleFor(x => x.OrganizationDto.Website)
            .NotEmpty().WithMessage("Website is required.")
            .Must(website => Uri.TryCreate(website, UriKind.RelativeOrAbsolute, out _))
            .WithMessage("Website is not a valid URL.");
    }
}

public class UpdateOrganizationCommandHandler(
    IUser user,
    IAuthorizationService authorizationService,
    IOrganizationService organizationService) : IRequestHandler<UpdateOrganizationCommand, Result>
{
    public async Task<Result> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult =
            await authorizationService.AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeOrganizationId(request.OrganizationDto.Id),
                Operations.Update);
        Guard.Against.UnAuthorized(authorizationResult);

        Result result = await organizationService.UpdateOrganization(request.OrganizationDto, cancellationToken);
        return result;
    }
}
