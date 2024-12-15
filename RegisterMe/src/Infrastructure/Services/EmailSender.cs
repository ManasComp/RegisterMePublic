#region

using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using RegisterMe.Application.BackgroundWorkers;
using RegisterMe.Application.Interfaces;
using RegisterMe.Application.RegistrationToExhibition.Dtos;

#endregion

namespace RegisterMe.Infrastructure.Services;

public class EmailSender : IEmailSender
{
    private readonly BackgroundTaskRunner _backgroundTaskRunner;
    private readonly string _emailFrom;
    private readonly string _host;
    private readonly string _password;
    private readonly int _port;

    public EmailSender(BackgroundTaskRunner backgroundTaskRunner, IConfiguration configuration)
    {
        string? email = configuration.GetValue<string>("Email:Mail");
        string? password = configuration.GetValue<string>("Email:Password");
        string? host = configuration.GetValue<string>("Email:Host");
        int? port = configuration.GetValue<int>("Email:Port");

        _backgroundTaskRunner = backgroundTaskRunner;
        _emailFrom = email ?? throw new ArgumentException("Email:Mail not found");
        _password = password ?? throw new ArgumentException("Email:Password not found");
        _host = host ?? throw new ArgumentException("Email:Host not found");
        _port = port ?? throw new ArgumentException("Email:Port not found");
    }

    public Task SendEmailAsync(string mainAddressToSendTo, string subject, string htmlMessage,
        CancellationToken cancellationToken = default)
    {
        MailMessage message = new();
        SendMail(mainAddressToSendTo, subject, htmlMessage, message, cancellationToken);
        return Task.CompletedTask;
    }

    public Task SendEmailAsync(string mainAddressToSendTo, string subject, string htmlMessage,
        string copyToSendTo, List<Invoice> attachments, CancellationToken cancellationToken = default)
    {
        MailMessage message = new();
        message.Bcc.Add(new MailAddress(copyToSendTo));
        foreach (Attachment? attachment in attachments.Select(attachment =>
                     new Attachment(attachment.Stream, attachment.FileName)))
        {
            message.Attachments.Add(attachment);
        }

        htmlMessage +=
            "\n Informace jsou pouze informativní, v případě nesrovnalostí má vždy pravdu organizace, která výstavu pořádá. Kontrolujte její propozice.";
        SendMail(mainAddressToSendTo, subject, htmlMessage, message, cancellationToken);
        attachments.ForEach(attachment => attachment.Stream.Position = 0);
        return Task.CompletedTask;
    }

    private void SendMail(string mainAddressToSendTo, string subject, string htmlMessage, MailMessage message,
        CancellationToken cancellationToken = default)
    {
        message.From = new MailAddress(_emailFrom);
        message.Subject = subject;
        message.To.Add(new MailAddress(mainAddressToSendTo));
        message.Body = htmlMessage;
        message.IsBodyHtml = true;
        SmtpClient smtpClient = new(_host)
        {
            Port = _port, Credentials = new NetworkCredential(_emailFrom, _password), EnableSsl = true
        };

        SendMail(message, smtpClient, cancellationToken);
    }

    private void SendMail(MailMessage message, SmtpClient smtpClient, CancellationToken cancellationToken = default)
    {
        _backgroundTaskRunner.FireRetryAndForgetTaskAsync(async () =>
        {
            await smtpClient.SendMailAsync(message, cancellationToken);
        }, cancellationToken);
    }
}
