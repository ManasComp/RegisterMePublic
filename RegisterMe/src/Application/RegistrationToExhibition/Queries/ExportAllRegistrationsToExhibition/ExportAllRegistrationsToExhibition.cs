#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Services.Converters;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Queries.ExportAllRegistrationsToExhibition;

// ReSharper disable always UnusedType.Global
public record ExportAllRegistrationsToExhibitionQuery : IRequest<string>
{
    public required int ExhibitionId { get; init; }
}

public class
    ExportAllRegistrationsToExhibitionQueryValidator : AbstractValidator<ExportAllRegistrationsToExhibitionQuery>
{
    public ExportAllRegistrationsToExhibitionQueryValidator()
    {
        RuleFor(v => v.ExhibitionId).ForeignKeyValidator();
    }
}

public class ExportAllRegistrationsToExhibitionQueryHandler(
    IAuthorizationService authorizationService,
    IUser user,
    IJsonExporterService jsonExporterService) : IRequestHandler<ExportAllRegistrationsToExhibitionQuery, string>
{
    public async Task<string> Handle(ExportAllRegistrationsToExhibitionQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitionId(request.ExhibitionId),
                Operations.DoOrganizationAdminStuff);
        Guard.Against.UnAuthorized(authorizationResult);

        string memoryStream =
            await jsonExporterService.GetDataAsync(request.ExhibitionId);
        return memoryStream;
    }
}
