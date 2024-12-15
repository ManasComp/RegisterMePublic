#region

using RegisterMe.Application.RegistrationToExhibition.Dtos;

#endregion

namespace RegisterMe.Application.Interfaces;

public interface IEmailSender
{
    /// <summary>
    ///     Send email async without attachments
    /// </summary>
    /// <param name="mainAddressToSendTo"></param>
    /// <param name="subject"></param>
    /// <param name="htmlMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendEmailAsync(string mainAddressToSendTo, string subject, string htmlMessage,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Send email async with attachments
    /// </summary>
    /// <param name="mainAddressToSendTo"></param>
    /// <param name="subject"></param>
    /// <param name="htmlMessage"></param>
    /// <param name="copyToSendTo"></param>
    /// <param name="attachments"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendEmailAsync(string mainAddressToSendTo, string subject, string htmlMessage,
        string copyToSendTo, List<Invoice> attachments, CancellationToken cancellationToken = default);
}
