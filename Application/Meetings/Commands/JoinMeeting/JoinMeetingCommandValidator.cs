using FluentValidation;

namespace Application.Meetings.Commands.JoinMeeting;

public class JoinMeetingCommandValidator : AbstractValidator<JoinMeetingCommand>
{
    public JoinMeetingCommandValidator()
    {
        RuleFor(x => x.MeetingId)
            .NotEmpty();
    }
}