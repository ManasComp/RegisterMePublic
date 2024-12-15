#region

using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition;

public interface IRegistrationToExhibitionService
{
    /// <summary>
    ///     Change advertisement
    /// </summary>
    /// <param name="advertisementId"></param>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> ChangeAdvertisement(int advertisementId, int registrationToExhibitionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates time when last notification was sent
    /// </summary>
    /// <param name="emails"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> UpdateSendNotifications(List<SimpleTypedEmail> emails,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Request online payment
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="paymentType"></param>
    /// <param name="currency"></param>
    /// <param name="amount"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result> RequestDelayedPayment(int registrationToExhibitionId, PaymentType paymentType,
        Currency currency,
        decimal amount,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get temporary registrations emails
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<List<TypedEmail>> GetTemporaryRegistrationsEmails(
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finish online payment
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="sessionId"></param>
    /// <param name="paymentIntentId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> FinishOnlinePayment(int registrationToExhibitionId, string sessionId,
        string paymentIntentId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finish delayed payment
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> FinishDelayedPayment(int registrationToExhibitionId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Starts online payment
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="sessionId"></param>
    /// <param name="currency"></param>
    /// <param name="amount"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result> StartOnlinePayment(int registrationToExhibitionId, string sessionId, Currency currency,
        decimal amount,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes registration
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> DeleteRegistration(int registrationToExhibitionId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Delete temporary registrations
    /// </summary>
    /// <param name="exhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<List<TypedEmail>>> DeleteTemporaryRegistrations(int? exhibitionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create registration to exhibition
    /// </summary>
    /// <param name="registrationToExhibitionDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<int>> CreateRegistrationToExhibition(CreateRegistrationToExhibitionDto registrationToExhibitionDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get registrations to exhibition by exhibitor id
    /// </summary>
    /// <param name="exhibitorId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<RegistrationToExhibitionDto>> GetRegistrationsToExhibitionByExhibitorId(int exhibitorId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get information if all ems are valid
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> HasValidEms(int registrationToExhibitionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get registrations to exhibition by exhibition id
    /// </summary>
    /// <param name="exhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<RegistrationToExhibitionDto>> GetRegistrationsToExhibitionByExhibitionId(int exhibitionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get registration to exhibition by exhibitor id and exhibition id
    /// </summary>
    /// <param name="exhibitionId"></param>
    /// <param name="exhibitorId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<RegistrationToExhibitionDto?> GetRegistrationToExhibitionByExhibitorIdAndExhibitionId(
        int exhibitionId, int exhibitorId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get registration to exhibition by id
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<RegistrationToExhibitionDto> GetRegistrationToExhibitionById(int registrationToExhibitionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Has active registrations
    /// </summary>
    /// <param name="exhibitionId"></param>
    /// <param name="userId"></param>
    /// <param name="paid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> HasActiveRegistrations(int exhibitionId, string userId, bool? paid,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Has active registrations
    /// </summary>
    /// <param name="exhibitionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> HasDrafts(int exhibitionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Balance the payment
    /// </summary>
    /// <param name="registrationToExhibitionId"></param>
    /// <param name="price"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result> BalanceThePayment(int registrationToExhibitionId, decimal price,
        CancellationToken cancellationToken = default);
}
