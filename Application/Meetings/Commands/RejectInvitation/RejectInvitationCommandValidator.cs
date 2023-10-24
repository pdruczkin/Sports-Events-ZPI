using FluentValidation;

namespace Application.Meetings.Commands.RejectInvitation;

public class RejectInvitationCommandValidator : AbstractValidator<RejectInvitationCommand>
{
    public RejectInvitationCommandValidator()
    {
        RuleFor(x => x.MeetingId)
            .NotEmpty();
    }
}