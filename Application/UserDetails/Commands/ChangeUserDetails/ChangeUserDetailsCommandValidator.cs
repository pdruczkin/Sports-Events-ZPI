using System.Security.Cryptography.Xml;
using Application.Common.Interfaces;
using FluentValidation;

namespace Application.UserDetails.Commands.ChangeUserDetails;

public class ChangeUserDetailsCommandValidator : AbstractValidator<ChangeUserDetailsCommand>
{
    public ChangeUserDetailsCommandValidator(IApplicationDbContext dbContext)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(256);
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(256);
        
        RuleFor(u => u.Email)
            .NotEmpty()
            .MaximumLength(256)
            .EmailAddress();

        RuleFor(u => u.Username)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(u => u.Gender).IsInEnum();
    }    
}