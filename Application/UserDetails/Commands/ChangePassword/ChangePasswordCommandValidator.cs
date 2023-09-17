using FluentValidation;

namespace Application.UserDetails.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty();
        
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(6)
            .Matches(@"[A-Z]+").WithMessage("Password must contain uppercase letter")
            .Matches(@"[a-z]+").WithMessage("Password must contain lowercase letter")
            .Matches(@"[0-9]+").WithMessage("Password must contain number");

        RuleFor(u => u.ConfirmPassword)
            .NotEmpty()
            .Equal(u => u.NewPassword).WithMessage("Confirm password must be equal to password");
    }
}