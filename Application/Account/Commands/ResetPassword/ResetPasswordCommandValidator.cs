﻿using FluentValidation;

namespace Application.Account.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Token)
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