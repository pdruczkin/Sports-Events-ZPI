using FluentValidation;

namespace Application.Meetings.Commands.RemoveUserFromMeeting;

public class RemoveUserFromMeetingCommandValidator : AbstractValidator<RemoveUserFromMeetingCommand>
{
    public RemoveUserFromMeetingCommandValidator()
    {
        RuleFor(x => x.MeetingId)
            .NotEmpty();
        
        RuleFor(x => x.UserToRemoveId)
            .NotEmpty();
    }
}