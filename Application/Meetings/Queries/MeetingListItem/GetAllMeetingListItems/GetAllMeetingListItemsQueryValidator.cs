using Application.Meetings.Queries.MeetingListItem.GetAllMeetingListItems;
using FluentValidation;

namespace Application.Meetings.Queries.MeetingPin.GetAllMeetingListItems;

public class GetAllMeetingListItemsQueryValidator : AbstractValidator<GetAllMeetingListItemsQuery>
{
    public GetAllMeetingListItemsQueryValidator()
    {
        RuleFor(x => x.SouthWestLatitude)
            .NotEmpty()
            .GreaterThanOrEqualTo(-90.0)
            .LessThanOrEqualTo(90.0)
            .WithMessage("Lattitude value out of range from (-90 to 90).");

        RuleFor(x => x.SouthWestLongitude)
            .NotEmpty()
            .GreaterThanOrEqualTo(-180.0)
            .LessThanOrEqualTo(180.0)
            .WithMessage("Longitude value out of range from (-180 to 180).");

        RuleFor(x => x.NorthEastLatitude)
            .NotEmpty()
            .GreaterThanOrEqualTo(-90.0)
            .LessThanOrEqualTo(90.0)
            .WithMessage("Lattitude value out of range from (-90 to 90).");

        RuleFor(x => x.NorthEastLongitude)
            .NotEmpty()
            .GreaterThanOrEqualTo(-180.0)
            .LessThanOrEqualTo(180.0)
            .WithMessage("Longitude value out of range from (-180 to 180).");
    }
}
