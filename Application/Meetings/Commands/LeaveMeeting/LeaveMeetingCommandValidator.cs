using FluentValidation;

namespace Application.Meetings.Commands.LeaveMeeting;

public class LeaveMeetingCommandValidator : AbstractValidator<LeaveMeetingCommand>
{
    public LeaveMeetingCommandValidator()
    {
        RuleFor(x => x.MeetingId)
            .NotEmpty();
    }
}