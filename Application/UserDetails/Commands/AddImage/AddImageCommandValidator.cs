using FluentValidation;

namespace Application.UserDetails.Commands.AddImage;

public class AddImageCommandValidator : AbstractValidator<AddImageCommand>
{
    public AddImageCommandValidator()
    {
        RuleFor(x => x.File)
            .NotNull();
        
        RuleFor(x => x.File.Length)
            .NotNull()
            .Must(x => x.Equals("image/jpeg") || x.Equals("image/jpg") || x.Equals("image/png"))
            .WithMessage("File type is larger than allowed");
    }
}