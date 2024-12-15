#region

using RegisterMe.Application.Interfaces;
using RegisterMe.Application.RegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.Services.Converters;
using Type = RegisterMe.Application.RegistrationToExhibition.Enums.Type;

#endregion

namespace RegisterMe.Application.BackgroundWorkers;

public class NotifierService(IRegistrationToExhibitionService registrationToExhibitionService, IEmailSender emailSender)
{
    private string? _thisWebUrl;

    public async Task NotifyRegistrations(string webUrl, CancellationToken cancellationToken = default)
    {
        List<TypedEmail> emails =
            await registrationToExhibitionService.GetTemporaryRegistrationsEmails(cancellationToken);
        List<TypedEmail> filteredEmails =
            emails.Where(e => e.Type == Type.IsDeleted || e.LastNotificationSendOn == null).ToList();
        await NotifyRegistrations(webUrl, filteredEmails, cancellationToken);
        List<TypedEmail> warningSendTo = filteredEmails.Where(e => e.Type == Type.CanBeDeletedInFuture).ToList();
        await registrationToExhibitionService.UpdateSendNotifications(
            warningSendTo.Select(e => new SimpleTypedEmail(e.Email, e.ExhibitionId))
                .ToList(), cancellationToken);
    }

    public async Task NotifyRegistrations(string webUrl, List<TypedEmail> emails,
        CancellationToken cancellationToken = default)
    {
        _thisWebUrl = webUrl;
        var messages = emails.Select(e => new
        {
            email = e.Email,
            message = e.Type switch
            {
                Type.IsDeleted => GetMessageDeleted(e.ExhibitionId),
                Type.CanBeDeletedInFuture => GetMessageToBeDeleted(e.ExhibitionId),
                _ => throw new ArgumentOutOfRangeException()
            }
        }).ToList();
        IEnumerable<Task> tasks = messages.Select(message =>
            emailSender.SendEmailAsync(message.email, "Nedokončená registrace na výstavu koček", message.message,
                cancellationToken)
        );

        await Task.WhenAll(tasks);
    }

    private string GetMessageToBeDeleted(int exhibitionId)
    {
        InvoiceSenderService.MessageBuilder messageBuilder = new();
        messageBuilder.AddRow("Máte nedokončenou registraci na výstavu koček", InvoiceSenderService.Headers.H1)
            .AddEmptyRow()
            .AddRow("Dobrý den,")
            .AddEmptyRow()
            .AddRow(
                "Tento email Vám byl automaticky zaslán, protože jste začal/a registraci na výstavu koček, ale nedokončil/a jste ji.")
            .AddRow(
                "Pokud jste se rozhodl/a neúčastnit se výstavy, nebo jste se registroval/a jiným způsobem, tento email můžete ignorovat.")
            .AddEmptyRow()
            .AddRow("Pokud se chcete registrovat skrze systém, pokračujte v registraci kliknutím na následující odkaz:")
            .AddLink(GetPath(exhibitionId), GetPath(exhibitionId))
            .AddRow("V opačném případě bude Vaše nedokočená registrace smazána.")
            .AddEmptyRow()
            .AddRow("Děkujeme")
            .AddRow("Tým výstavy koček");
        string text = messageBuilder.Build();
        return text;
    }


    private string GetPath(int exhibitionId)
    {
        return _thisWebUrl + "Visitor/Exhibition/Detail/" + exhibitionId;
    }

    private string GetMessageDeleted(int exhibitionId)
    {
        InvoiceSenderService.MessageBuilder messageBuilder = new();
        messageBuilder.AddRow("Smazali jsme Vaši nedokončenou registraci na výstavu koček",
                InvoiceSenderService.Headers.H1)
            .AddEmptyRow()
            .AddRow("Dobrý den,")
            .AddEmptyRow()
            .AddRow(
                "Tento email Vám byl zaslán, protože jste začal/a registraci na výstavu koček, ale nedokončil/a jste ji.")
            .AddRow("Jakož to neaktivní byla Vaše registrace byla smazána.")
            .AddRow(
                "Pokud jste se rozhodl/a neúčastnit se výstavy, nebo jste se registroval/a jiným způsobem, tento email můžete ignorovat.")
            .AddEmptyRow()
            .AddRow(
                "Pokud se výstavy chcete účastnit a registrovat se skrze systém, zaregistrujte se prosím znovu na následujícím odkazu:")
            .AddLink(GetPath(exhibitionId), GetPath(exhibitionId))
            .AddEmptyRow()
            .AddRow("Děkujeme")
            .AddRow("Tým výstavy koček");
        string text = messageBuilder.Build();
        return text;
    }
}
