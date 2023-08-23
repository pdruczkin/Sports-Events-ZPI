using Application.Common.Interfaces;
using FluentValidation;

namespace Application.Account.Commands;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator(IApplicationDbContext dbContext)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(256);
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(256);
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .Matches(@"[A-Z]+").WithMessage("Password must contain uppercase letter")
            .Matches(@"[a-z]+").WithMessage("Password must contain lowercase letter")
            .Matches(@"[0-9]+").WithMessage("Password must contain number");

        RuleFor(u => u.ConfirmPassword)
            .NotEmpty()
            .Equal(u => u.Password).WithMessage("Confirm password must be equal to password");

        RuleFor(u => u.Email)
            .NotEmpty()
            .MaximumLength(256)
            .EmailAddress()
            .Custom((value, context) =>
            {
                if (dbContext.Users.Any(x => x.Email == value))
                {
                    context.AddFailure("Email", "Email must be unique");
                }
            });
        
        RuleFor(u => u.Username)
            .NotEmpty()
            .MaximumLength(256)
            .Custom((value, context) =>
            {
                if (dbContext.Users.Any(x => x.Username == value))
                {
                    context.AddFailure("Username", "Username must be unique");
                }
            });
        
    }
}