using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
