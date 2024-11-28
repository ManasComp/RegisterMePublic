#region

using System.Text;
using RegisterMe.Application.Exhibitions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Interfaces;
using RegisterMe.Application.Pricing;
using RegisterMe.Application.RegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Services.Converters;

public class InvoiceSenderService(
    IRegistrationToExhibitionService registrationToExhibitionService,
    IInvoiceCreator invoiceCreator,
    IEmailSender emailSender,
    IPricingFacade pricingFacade,
    IExhibitionService exhibitionService,
    IZipService zipService) : IInvoiceSenderService
{
    public enum Headers
    {
        H1,
        H2,
        H3,
        Text
    }

    public async Task SendRegistrationConfirmationInvoiceToMail(int registrationToExhibitionId, string webUrl,
        string rootPath)
    {
        RegistrationToExhibitionDto registrationToExhibition =
            await registrationToExhibitionService.GetRegistrationToExhibitionById(registrationToExhibitionId);

        BriefExhibitionDto exhibition =
            await exhibitionService.GetExhibitionById(registrationToExhibition.ExhibitionId);

        HashSet<Invoice> paths =
            await invoiceCreator.CreateRegistrationToExhibitionInvoice(registrationToExhibitionId, webUrl, rootPath);

        MessageBuilder messageBuilder = new();

        messageBuilder.AddRow("Přijmuli jsme Vaši žádost o registraci na výstavu.", Headers.H2);
        messageBuilder.AddRow(" V příloze zasíláme její kopii.", Headers.H3);

        if (registrationToExhibition.PaymentInfo != null &&
            RegistrationToExhibitionService.IsPaid(registrationToExhibition.PaymentInfo))
        {
            PaymentInfoDto? paymentInfo = registrationToExhibition.PaymentInfo;
            string amountWithCurrency = $"{paymentInfo.Amount} {paymentInfo.Currency}";
            if (registrationToExhibition.PaymentInfo.PaymentCompletedDate == null)
            {
                switch (paymentInfo.PaymentType)
                {
                    case PaymentType.PayInPlaceByCache:
                        messageBuilder.AddRow($"Platbu ve výši {amountWithCurrency} uhradíte na místě.");
                        break;

                    case PaymentType.PayByBankTransfer:
                        string beneficiaryMessage =
                            await pricingFacade.GetBeneficiaryMessage(registrationToExhibitionId);
                        messageBuilder
                            .AddRow("Platbu uhraďte na účet v přesném formátu, jako je znázorněno níže:")
                            .AddTable([
                                new RowWithHeader { Header = "IBAN", Value = [exhibition.Iban] },
                                new RowWithHeader { Header = "Účet", Value = [exhibition.BankAccount] },
                                new RowWithHeader
                                {
                                    Header = "Variabilní symbol", Value = [registrationToExhibitionId.ToString()]
                                },
                                new RowWithHeader { Header = "Cena", Value = [amountWithCurrency] },
                                new RowWithHeader { Header = "Zpráva pro příjemce", Value = [beneficiaryMessage] }
                            ]);

                        break;

                    case PaymentType.PayOnlineByCard:
                        messageBuilder.AddRow($"Platbu ve výši {amountWithCurrency} uhradíte online.");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                switch (paymentInfo.PaymentType)
                {
                    case PaymentType.PayInPlaceByCache:
                        messageBuilder.AddRow($"Platba ve výši {amountWithCurrency} byla přijata na místě.");
                        break;

                    case PaymentType.PayByBankTransfer:
                        messageBuilder.AddRow($"Platba ve výši {amountWithCurrency} byla přijata na účet.");
                        break;

                    case PaymentType.PayOnlineByCard:
                        messageBuilder.AddRow($"Platba ve výši {amountWithCurrency} byla přijata online.");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        messageBuilder.AddRow(
            "Tento email nepotvrzuje Vaše zapsání do výstavy, pouze přijetí žádosti. Vyčkejte na email od pořadatele výstavy.");

        string message = messageBuilder.Build();
        await emailSender.SendEmailAsync(
            registrationToExhibition.PersonRegistration.Email,
            "Registrace na výstavu",
            message,
            exhibition.Email,
            paths.ToList());
    }

    public async Task SendPaymentConfirmationInvoiceToMail(int registrationToExhibitionId, string webUrl,
        string rootPath)
    {
        RegistrationToExhibitionDto registrationToExhibition =
            await registrationToExhibitionService.GetRegistrationToExhibitionById(registrationToExhibitionId);
        BriefExhibitionDto exhibition =
            await exhibitionService.GetExhibitionById(registrationToExhibition.ExhibitionId);
        HashSet<Invoice> paths =
            await invoiceCreator.CreateRegistrationToExhibitionInvoice(registrationToExhibitionId, webUrl, rootPath);

        MessageBuilder messageBuilder = new();

        messageBuilder.AddRow("Potvrzení platby registrace na výstavu.", Headers.H2);
        messageBuilder.AddRow("V příloze zasíláme její kopii.", Headers.H3);
        messageBuilder.AddEmptyRow();
        messageBuilder.AddRow("Tímto Vám potvrzujeme přijetí platby za registraci na výstavu.");
        messageBuilder.AddEmptyRow();

        messageBuilder.AddRow(
            "Tento email nepotvrzuje Vaše zapsání do výstavy, pouze přijetí její platby. Vyčkejte na email od pořadatele výstavy.");

        await emailSender.SendEmailAsync(registrationToExhibition.PersonRegistration.Email,
            "Potvrzení platby registrace na výstavu",
            messageBuilder.Build(), exhibition.Email,
            paths.ToList());
    }

    public async Task<Stream> GetInvoicesByRegistrationZip(int registrationToExhibitionId, string webUrl,
        string rootPath)
    {
        HashSet<Invoice> paths =
            await invoiceCreator.CreateRegistrationToExhibitionInvoice(registrationToExhibitionId, webUrl, rootPath);

        Stream result = await zipService.CreateZipAsync(paths);
        return result;
    }

    public async Task<Stream> GetInvoicesByExhibitionZip(int exhibitionId, string webUrl, string rootPath)
    {
        List<List<Invoice>> paths = await invoiceCreator.CreateExhibitionInvoice(exhibitionId, webUrl, rootPath);
        Stream result = await zipService.CreateZipAsync(paths.SelectMany(x => x).ToList());
        return result;
    }

    public class MessageBuilder
    {
        private readonly StringBuilder _message = new();

        public MessageBuilder AddRow(string row, Headers headers = Headers.Text)
        {
            string formattedeRow = headers switch
            {
                Headers.H1 => $"<h1>{row}</h1>",
                Headers.H2 => $"<h2>{row}</h2>",
                Headers.H3 => $"<h3>{row}</h3>",
                Headers.Text => $"<p>{row}</p>",
                _ => throw new ArgumentOutOfRangeException()
            };
            _message.Append(formattedeRow);
            return this;
        }

        public MessageBuilder AddEmptyRow()
        {
            _message.Append("<br>");
            return this;
        }

        public MessageBuilder AddTable(List<RowWithHeader> rows)
        {
            _message.Append("<table>");
            foreach (RowWithHeader row in rows)
            {
                _message.Append("<tr>");
                _message.Append($"<th>{row.Header}</th>");
                foreach (string value in row.Value)
                {
                    _message.Append($"<td>{value}</td>");
                }

                _message.Append("</tr>");
            }

            _message.Append("</table>");
            return this;
        }

        public MessageBuilder AddLink(string link, string text)
        {
            _message.Append($"<a href='{link}'>{text}</a>");
            return this;
        }

        public MessageBuilder AddImage(string src, string alt)
        {
            _message.Append($"<img src='{src}' alt='{alt}'>");
            return this;
        }

        public string Build()
        {
            return _message.ToString();
        }
    }

    public record RowWithHeader
    {
        public string Header { get; init; } = null!;
        public List<string> Value { get; init; } = null!;
    }
}
