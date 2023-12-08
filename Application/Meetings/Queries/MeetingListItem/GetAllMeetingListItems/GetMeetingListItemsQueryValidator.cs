using Application.Meetings.Queries.MeetingListItem.GetAllMeetingListItems;
using Domain.Entities;
using FluentValidation;

namespace Application.Meetings.Queries.MeetingPin.GetAllMeetingListItems;

public class GetMeetingListItemsQueryValidator : AbstractValidator<GetMeetingListItemsQuery>
{
    private string[] allowedSortByColumnNames =
    {
        nameof(Meeting.StartDateTimeUtc),
        nameof(Meeting.Difficulty),
        nameof(Meeting.MaxParticipantsQuantity)
    };

    public GetMeetingListItemsQueryValidator()
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

        RuleFor(x => x.StartDateTimeUtc)
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Search allowed for future meetings only.");

        RuleFor(x => x.SportsDiscipline)
            .IsInEnum();

        RuleFor(x => x.Difficulty)
            .IsInEnum();

        RuleFor(r => r.SortBy).Must(value => string.IsNullOrEmpty(value) || allowedSortByColumnNames.Contains(value))
            .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");

    }
}
