#region

using RegisterMe.Application.Interfaces;
using RegisterMe.Application.RegistrationToExhibition.Dtos;

#endregion

namespace RegisterMe.Application.FunctionalTests;

#region

using Dtos_Invoice = Invoice;

#endregion

public class FakeEmailSender : IEmailSender
{
    public Task SendEmailAsync(string mainAddressToSendTo, string subject, string htmlMessage,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task SendEmailAsync(string mainAddressToSendTo, string subject, string htmlMessage, string copyToSendTo,
        List<Dtos_Invoice> attachments, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
