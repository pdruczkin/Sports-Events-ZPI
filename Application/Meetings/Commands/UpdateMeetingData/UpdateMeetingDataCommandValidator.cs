using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Meetings.Commands.UpdateMeetingData;

public class UpdateMeetingDataCommandValidator : AbstractValidator<UpdateMeetingDataCommand>
{
    public UpdateMeetingDataCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Description)
            .NotNull()
            .MaximumLength(8192);

        RuleFor(x => x.Latitude)
            .NotEmpty()
            .GreaterThanOrEqualTo(-90.0)
            .LessThanOrEqualTo(90.0)
            .WithMessage("Lattitude value out of range from (-90 to 90).");

        RuleFor(x => x.Longitude)
            .NotEmpty()
            .GreaterThanOrEqualTo(-180.0)
            .LessThanOrEqualTo(180.0)
            .WithMessage("Longitude value out of range from (-180 to 180).");

        RuleFor(x => x.StartDateTimeUtc)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Updated meeting's start time can not be in the past.")
            .LessThan(x => x.EndDateTimeUtc)
            .WithMessage("Meeting's start time must be before meeting's end time.");

        RuleFor(x => x.EndDateTimeUtc)
            .NotEmpty()
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Updated meeting's end time can not be in the past.");

        RuleFor(x => x.Visibility)
            .NotNull()
            .IsInEnum();

        RuleFor(x => x.SportsDiscipline)
            .NotNull()
            .IsInEnum();

        RuleFor(x => x.Difficulty)
            .NotNull()
            .IsInEnum();

        RuleFor(x => x.MaxParticipantsQuantity)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Min participants quantity is 1.")
            .LessThanOrEqualTo(100)
            .WithMessage("Max participants quantity is 100.");

        RuleFor(x => x.MinParticipantsAge)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Min participants age is 1.")
            .LessThanOrEqualTo(130)
            .WithMessage("Max participants age is 130.");
    }
}
