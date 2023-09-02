using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IEmailSender
{
    void SendEmail(EmailDto request);
}