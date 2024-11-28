namespace RegisterMe.Application.Services.Converters;

public interface IInvoiceDataProvider
{
    public Task<List<InvoiceModel>> GetInvoiceModel(int catRegistrationId, string serverUrl);
}
