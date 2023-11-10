using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.Common.ExtensionMethods;

public static class ExtensionMethods
{
    public static int CalculateAge(this DateTime dateOfBirth)
    {
        var age = DateTime.Now.Year - dateOfBirth.Year;
        if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
            age--;

        return age;
    }

    public static int CountMeetingParticipantsQuantity(this IApplicationDbContext dbContext, Guid meetingId)
    {
        var totalParticipantsQuantity = 1 + dbContext // 1 - meeting's organizer is also a participant
            .MeetingParticipants
            .Count(mp => mp.MeetingId == meetingId && mp.InvitationStatus == InvitationStatus.Accepted);
        return totalParticipantsQuantity;
    }
}
