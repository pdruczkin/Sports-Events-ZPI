using FluentValidation;

namespace Application.UserDetails.Commands.DeleteImage;

public class DeleteImageCommandValidator : AbstractValidator<DeleteImageCommand>
{
    public DeleteImageCommandValidator()
    {
        RuleFor(x => x.PublicId)
            .NotEmpty();
    }
}