using Application.Meetings.Queries.MeetingListItem.GetAllMeetingListItems;
using Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Meetings.Queries.MeetingHistory;

public class GetMeetingHistoryQueryValidator : AbstractValidator<GetMeetingsHistoryQuery>
{
    private int[] allowedPageSizes = new[] { 10, 30, 60, 120 };
    private string[] allowedSortByColumnNames =
    { 
        nameof(Meeting.StartDateTimeUtc), 
        nameof(Meeting.Difficulty), 
        nameof(Meeting.MaxParticipantsQuantity),
        nameof(Meeting.StartDateTimeUtc)
    };

    public GetMeetingHistoryQueryValidator()
    {
        RuleFor(x => x.StartDateTimeUtcFrom)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Search allowed for past meetings only.");

        RuleFor(x => x.StartDateTimeUtcTo)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Search allowed for past meetings only.");

        RuleFor(x => x.SportsDiscipline)
            .IsInEnum();

        RuleFor(x => x.Difficulty)
            .IsInEnum();

        RuleFor(x => x.MeetingVisibility)
            .IsInEnum();

        RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);

        RuleFor(r => r.PageSize).Custom((value, context) =>
        {
            if (!allowedPageSizes.Contains(value))
            {
                context.AddFailure("PageSize", $"PageSize must in [{string.Join(",", allowedPageSizes)}]");
            }
        });

        RuleFor(r => r.SortBy).Must(value => string.IsNullOrEmpty(value) || allowedSortByColumnNames.Contains(value))
            .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");
    }
}