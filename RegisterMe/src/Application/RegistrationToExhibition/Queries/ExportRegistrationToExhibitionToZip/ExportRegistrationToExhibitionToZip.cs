#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.Services.Converters;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Queries.ExportRegistrationToExhibitionToZip;

// ReSharper disable always UnusedType.Global
public record ExportRegistrationToExhibitionToZipQuery : IRequest<Stream>
{
    public required int Id { get; init; }
    public required string WebUrl { get; init; }
    public required string RootPath { get; init; }
}

public class
    ExportRegistrationToExhibitionToZipQueryValidator : AbstractValidator<ExportRegistrationToExhibitionToZipQuery>
{
    public ExportRegistrationToExhibitionToZipQueryValidator()
    {
        RuleFor(v => v.Id).ForeignKeyValidator();
        RuleFor(v => v.WebUrl).Length(1, 1000);
    }
}

public class ExportRegistrationToExhibitionToZipQueryHandler(
    IAuthorizationService authorizationService,
    IUser user,
    IInvoiceSenderService invoiceSenderService) : IRequestHandler<ExportRegistrationToExhibitionToZipQuery, Stream>
{
    public async Task<Stream> Handle(ExportRegistrationToExhibitionToZipQuery request,
        CancellationToken cancellationToken)
    {
        AuthorizationResult authorizationResult = await authorizationService
            .AuthorizeAsync(AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
                new AuthorizeRegistrationToExhibitionId(request.Id), Operations.Read);
        Guard.Against.UnAuthorized(authorizationResult);

        Stream memoryStream =
            await invoiceSenderService.GetInvoicesByRegistrationZip(request.Id, request.WebUrl, request.RootPath);
        return memoryStream;
    }
}
