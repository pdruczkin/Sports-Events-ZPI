using FluentValidation;

namespace Application.Friends.Commands.SendFriendInvitation;

public class SendFriendInvitationCommandValidator : AbstractValidator<SendFriendInvitationCommand>
{
    public SendFriendInvitationCommandValidator()
    {
        RuleFor(x => x.InviteeId)
            .NotEmpty();
    }
}
