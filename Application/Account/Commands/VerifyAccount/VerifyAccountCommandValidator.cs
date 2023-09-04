using FluentValidation;

namespace Application.Account.Commands.VerifyAccount;

public class VerifyAccountCommandValidator : AbstractValidator<VerifyAccountCommand>
{
    public VerifyAccountCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();
    }
}