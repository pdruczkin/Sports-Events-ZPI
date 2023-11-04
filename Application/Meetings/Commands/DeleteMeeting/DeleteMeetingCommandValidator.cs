using FluentValidation;

namespace Application.Meetings.Commands.DeleteMeeting;

public class DeleteMeetingCommandValidator : AbstractValidator<DeleteMeetingCommand>
{
    public DeleteMeetingCommandValidator()
    {
        RuleFor(x => x.MeetingId).NotEmpty();
    }
}
