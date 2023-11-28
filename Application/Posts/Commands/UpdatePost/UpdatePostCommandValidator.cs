using FluentValidation;

namespace Application.Posts.Commands.UpdatePost;

public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
{
    public UpdatePostCommandValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty();
        
        RuleFor(x => x.Title)
            .NotEmpty();
        
        RuleFor(x => x.Text)
            .NotEmpty();
    }
}