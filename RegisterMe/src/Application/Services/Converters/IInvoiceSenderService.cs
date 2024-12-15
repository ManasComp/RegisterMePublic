namespace RegisterMe.Application.Services.Converters;

public interface IInvoiceSenderService
{
    Task SendRegistrationConfirmationInvoiceToMail(int registrationToExhibitionId, string webUrl, string rootPath);
    Task SendPaymentConfirmationInvoiceToMail(int registrationToExhibitionId, string webUrl, string rootPath);
    Task<Stream> GetInvoicesByRegistrationZip(int registrationToExhibitionId, string webUrl, string rootPath);
    Task<Stream> GetInvoicesByExhibitionZip(int exhibitionId, string webUrl, string rootPath);
}
