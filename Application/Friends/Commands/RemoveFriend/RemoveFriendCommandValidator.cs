using FluentValidation;

namespace Application.Friends.Commands.RemoveFriend;

public class RemoveFriendCommandValidator : AbstractValidator<RemoveFriendCommand>
{
    public RemoveFriendCommandValidator()
    {
        RuleFor(x => x.FriendToRemoveId).NotEmpty();
    }
}
