using FluentValidation;

namespace Application.Friends.Commands.SendFriendInvitation;

public class SendFriendInvitationCommandValidator : AbstractValidator<SendFriendInvitationCommand>
{
    public SendFriendInvitationCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(256);
    }
}
