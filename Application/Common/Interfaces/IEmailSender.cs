using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(EmailDto request);
}