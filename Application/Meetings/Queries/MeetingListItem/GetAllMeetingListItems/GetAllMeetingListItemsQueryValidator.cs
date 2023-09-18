using Application.Meetings.Queries.MeetingListItem.GetAllMeetingListItems;
using FluentValidation;

namespace Application.Meetings.Queries.MeetingPin.GetAllMeetingListItems;

public class GetAllMeetingListItemsQueryValidator : AbstractValidator<GetMeetingListItemsQuery>
{
    public GetAllMeetingListItemsQueryValidator()
    {
        RuleFor(x => x.SouthWestLatitude)
            .NotNull()
            .GreaterThanOrEqualTo(-90.0)
            .LessThanOrEqualTo(90.0)
            .WithMessage("Lattitude value out of range from (-90 to 90).");

        RuleFor(x => x.SouthWestLongitude)
            .NotNull()
            .GreaterThanOrEqualTo(-180.0)
            .LessThanOrEqualTo(180.0)
            .WithMessage("Longitude value out of range from (-180 to 180).");

        RuleFor(x => x.NorthEastLatitude)
            .NotNull()
            .GreaterThanOrEqualTo(-90.0)
            .LessThanOrEqualTo(90.0)
            .WithMessage("Lattitude value out of range from (-90 to 90).");

        RuleFor(x => x.NorthEastLongitude)
            .NotNull()
            .GreaterThanOrEqualTo(-180.0)
            .LessThanOrEqualTo(180.0)
            .WithMessage("Longitude value out of range from (-180 to 180).");

        RuleFor(x => x.SportsDiscipline)
            .IsInEnum();

        RuleFor(x => x.Difficulty)
            .IsInEnum();

        RuleFor(x => x.MeetingVisibility)
            .IsInEnum();
    }
}
