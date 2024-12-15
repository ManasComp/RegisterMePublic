#region

using RegisterMe.Application.Pricing.Dtos;
using RegisterMe.Application.Pricing.Enums;

#endregion

namespace RegisterMe.Application.Pricing;

public interface IPricingFacade
{
    /// <summary>
    ///     Get price
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<RegistrationToExhibitionPrice> GetPrice(int registrationToExhibitionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get beneficiary message
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <returns></returns>
    Task<string> GetBeneficiaryMessage(int registrationToExhibitionId);

    /// <summary>
    ///     Get available payment types
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <returns></returns>
    Task<List<PaymentTypeWithCurrency>> GetAvailablePaymentTypes(int registrationToExhibitionId);
}
