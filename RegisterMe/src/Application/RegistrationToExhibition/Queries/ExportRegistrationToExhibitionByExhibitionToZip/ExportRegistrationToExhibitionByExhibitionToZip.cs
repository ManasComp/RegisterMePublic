#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Services.Converters;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Queries.ExportRegistrationToExhibitionByExhibitionToZip;

// ReSharper disable always UnusedType.Global
public record ExportRegistrationToExhibitionByExhibitionToZipQuery : IRequest<Stream>
{
    public required int Id { get; init; }
    public required string WebUrl { get; init; }
    public required string RootPath { get; init; }
}

public class
    ExportRegistrationToExhibitionToZipByExhibitionQueryValidator : AbstractValidator<
    ExportRegistrationToExhibitionByExhibitionToZipQuery>
{
    public ExportRegistrationToExhibitionToZipByExhibitionQueryValidator()
    {
        RuleFor(v => v.Id).ForeignKeyValidator();
        RuleFor(v => v.WebUrl).Length(1, 1000);
    }
}

public class ExportRegistrationToExhibitionToZipByExhibitionQueryHandler(
    IAuthorizationService authorizationService,
    IUser user,
    IInvoiceSenderService invoiceSenderService)
    : IRequestHandler<ExportRegistrationToExhibitionByExhibitionToZipQuery, Stream>
{
    public async Task<Stream> Handle(ExportRegistrationToExhibitionByExhibitionToZipQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeExhibitionId(request.Id), Operations.DoOrganizationAdminStuff);
        Guard.Against.UnAuthorized(authorizationResult);

        Stream memoryStream =
            await invoiceSenderService.GetInvoicesByExhibitionZip(request.Id, request.WebUrl, request.RootPath);
        return memoryStream;
    }
}
