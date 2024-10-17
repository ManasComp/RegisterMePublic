namespace RegisterMe.Application.Services.Converters;

public interface IInvoiceSenderService
{
    Task SendRegistrationConfirmationInvoiceToMail(int registrationToExhibitionId, string webUrl, string rootPath);
    Task SendPaymentConfirmationInvoiceToMail(int registrationToExhibitionId, string webUrl, string rootPath);
    Task<Stream> GetInvoicesZip(int registrationToExhibitionId, string webUrl, string rootPath);
}
