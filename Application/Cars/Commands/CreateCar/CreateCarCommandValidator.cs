using FluentValidation;

namespace Application.Cars.Commands.CreateCar
{
    public class CreateCarCommandValidator : AbstractValidator<CreateCarCommand>
    {
        public CreateCarCommandValidator()
        {
            RuleFor(x => x.MaxSpeed)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(x => x.Color)
                .NotEmpty();
        }
    }
}
