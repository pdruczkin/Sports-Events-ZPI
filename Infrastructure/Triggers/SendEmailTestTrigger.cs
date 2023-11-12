using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using EntityFrameworkCore.Triggered;

namespace Infrastructure.Triggers;

public class SendEmailTestTrigger : IAfterSaveTrigger<Meeting>
{
    private readonly IEmailSender _emailSendService;

    public SendEmailTestTrigger(IEmailSender emailSendService)
    {
        _emailSendService = emailSendService;
    }

    public async Task AfterSave(ITriggerContext<Meeting> context, CancellationToken cancellationToken)
    {
        if (context.ChangeType == ChangeType.Added)
        { 
            await _emailSendService.SendEmailAsync(new EmailDto() {
                To = "260369@student.pwr.edu.pl",
                Body = "TEST EMAIL - tekst",
                Subject = "TEST EMAIL"
            }); // Initial email
        }
        /*
        else if (context.ChangeType == ChangeType.Modified && context.Entity.Message != context.UnmodifiedEntity.Message)
        {
            _emailSendService.Send(email); // In case the content was updated we want to resent this email
        }*/

        return;// Task.FromResult();
    }
}