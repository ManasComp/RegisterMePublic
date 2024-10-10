#region

using RegisterMe.Application.RegistrationToExhibition.Dtos;

#endregion

namespace RegisterMe.Application.Services.Converters;

public interface IInvoiceCreator
{
    Task<HashSet<Invoice>> CreateRegistrationToExhibitionInvoice(int registrationToExhibitionId, string webUrl,
        string rootPath);
}
