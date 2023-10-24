using FluentValidation;

namespace Application.Friends.Commands.RejectFriendInvitation;

public class RejectFriendInvitationCommandValidator : AbstractValidator<RejectFriendInvitationCommand>
{
    public RejectFriendInvitationCommandValidator()
    {
        RuleFor(x => x.InviterId)
            .NotEmpty();
    }
}