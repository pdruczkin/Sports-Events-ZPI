using FluentValidation;

namespace Application.Friends.Commands.UnlockUser;

public class UnlockUserCommandValidator : AbstractValidator<UnlockUserCommand>
{
    public UnlockUserCommandValidator()
    {
        RuleFor(x => x.UserToUnlockId)
            .NotEmpty();
    }
}