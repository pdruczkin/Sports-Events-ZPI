using Application.Common.Interfaces;
using Application.Common.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Infrastructure.Services;

public class EmailSender : IEmailSender
{
    private readonly EmailSenderSettings _senderSettings;

    public EmailSender(EmailSenderSettings senderSettings)
    {
        _senderSettings = senderSettings;
    }
    
    public async Task SendEmailAsync(EmailDto request)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_senderSettings.EmailUsername));
        email.To.Add(MailboxAddress.Parse(request.To));
        email.Subject = request.Subject;
        email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_senderSettings.EmailHost, 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_senderSettings.EmailUsername, _senderSettings.EmailPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}