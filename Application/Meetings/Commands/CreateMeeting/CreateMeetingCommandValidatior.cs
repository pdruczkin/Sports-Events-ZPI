using FluentValidation;

namespace Application.Meetings.Commands.CreateMeeting;

public class CreateMeetingCommandValidatior :AbstractValidator<CreateMeetingCommand>
{
    public CreateMeetingCommandValidatior()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Description)
            .NotEmpty()
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
            .WithMessage("Start time of a new meeting can not be in the past.");

        RuleFor(x => x.StartDateTimeUtc)
            .LessThan(x => x.EndDateTimeUtc)
            .WithMessage("Meeting's start time must be before meeting's end time.");

        RuleFor(x => x.Visibility)
            .NotEmpty()
            .IsInEnum();

        RuleFor(x => x.SportsDiscipline)
            .NotEmpty()
            .IsInEnum();

        RuleFor(x => x.Difficulty)
            .NotEmpty()
            .IsInEnum();

    }
}
