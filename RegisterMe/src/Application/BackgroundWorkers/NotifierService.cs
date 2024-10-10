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
        await NotifyRegistrations(webUrl, emails, cancellationToken);
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
                "Tento email Vám byl zaslán, protože jste začali registraci na výstavu koček, ale neukončili jste ji.")
            .AddRow("Pokud jste se rozhodli neúčastnit se výstavy, tento email můžete ignorovat.")
            .AddEmptyRow()
            .AddRow("Pokud se chcete zúčastnit výstavy, pokračujte v registraci kliknutím na následující odkaz:")
            .AddLink(GetPath(exhibitionId), GetPath(exhibitionId))
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
                "Tento email Vám byl zaslán, protože jste začali registraci na výstavu koček, ale neukončili jste ji.")
            .AddRow("Jakož to neaktivní byla vaše registrace byla smazána.")
            .AddRow(
                "Pokud se výstavy účastnit nechcete, tento email můžete ignorovat a už od Vás nejsou vyžadovány žádné další akce.")
            .AddEmptyRow()
            .AddRow("Pokud se výstvy chcete účastnit, zaregistrujte se prosím znovu na následujícím odkazu:")
            .AddLink(GetPath(exhibitionId), GetPath(exhibitionId))
            .AddEmptyRow()
            .AddRow("Děkujeme")
            .AddRow("Tým výstavy koček");
        string text = messageBuilder.Build();
        return text;
    }
}
