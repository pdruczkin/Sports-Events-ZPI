using Domain.Entities;
using FluentValidation;

namespace Application.Meetings.Queries.UpcomingUserMeetings;

public class GetUpcomingMeetingsQueryValidator : AbstractValidator<GetUpcomingMeetingsQuery>
{
    private string[] allowedSortByColumnNames =
    { 
        nameof(Meeting.StartDateTimeUtc), 
        nameof(Meeting.Difficulty), 
        nameof(Meeting.MaxParticipantsQuantity),
        nameof(Meeting.StartDateTimeUtc)
    };

    public GetUpcomingMeetingsQueryValidator()
    {
        RuleFor(x => x.StartDateTimeUtcFrom)
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Search allowed for upcoming meetings only.");

        RuleFor(x => x.StartDateTimeUtcTo)
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Search allowed for upcoming meetings only.");

        RuleFor(x => x.SportsDiscipline)
            .IsInEnum();

        RuleFor(x => x.Difficulty)
            .IsInEnum();

        RuleFor(x => x.MeetingVisibility)
            .IsInEnum();

        RuleFor(r => r.SortBy).Must(value => string.IsNullOrEmpty(value) || allowedSortByColumnNames.Contains(value))
            .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");
    }
}