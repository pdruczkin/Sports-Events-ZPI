using Application.Friends.Commands.SendFriendInvitation;
using FluentValidation;

namespace Application.Friends.Commands.AcceptFriendInvitation;

public class AcceptFriendInvitationCommandValidator : AbstractValidator<AcceptFriendInvitationCommand>
{
    public AcceptFriendInvitationCommandValidator()
    {
        RuleFor(x => x.InviterId)
            .NotEmpty();
    }
}