using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Statistics.Queries;
using AutoMapper;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.ExtensionMethods;


public static class StatisticsExtensionMethods
{
    public static async Task<StatisticsDto> GetUserStatistics(this IApplicationDbContext dbContext, Guid? userId, IDateTimeProvider dateTimeProvider, IMapper mapper, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null) throw new AppException("User is not found");

        var allUserMeetings = await dbContext
            .Meetings
            .Include(x => x.MeetingParticipants).ThenInclude(x => x.Participant)
            .Include(x => x.Organizer)
            .Where(x => x.EndDateTimeUtc < dateTimeProvider.UtcNow && (x.OrganizerId == userId
                    || (x.MeetingParticipants.ToList().Any(mp => mp.ParticipantId == userId) && x.MeetingParticipants.FirstOrDefault(mp => mp.ParticipantId == userId)!.InvitationStatus == InvitationStatus.Accepted)))
            .ToListAsync(cancellationToken);

        var meetingsOrganizedCount = allUserMeetings.Count(x => x.OrganizerId == userId);

        var meetingsParticipatedCount = allUserMeetings.Count(x => x.OrganizerId != userId);

        var totalMinutesInMeetings = allUserMeetings.Sum(x => (int)(x.EndDateTimeUtc - x.StartDateTimeUtc).TotalMinutes);


        FavoriteDisciplineDto? favoriteSportDiscipline = null;
        int? avgParticipantsAge = null;
        FavoriteParticipantDto? favoriteParticipant = null;

        if (allUserMeetings.Any())
        {
            var favoriteSportDisciplineGroup = allUserMeetings
                .GroupBy(x => x.SportsDiscipline)
                .MaxBy(x => x.Count());

            favoriteSportDiscipline = new FavoriteDisciplineDto() {
                SportDiscipline = favoriteSportDisciplineGroup.Key.ToString(),
                Count = favoriteSportDisciplineGroup.Count()
            };

            avgParticipantsAge = (int)allUserMeetings
                .SelectMany(a => a.MeetingParticipants.Where(x => x.InvitationStatus == InvitationStatus.Accepted && x.ParticipantId != userId).Select(x => x.Participant!.DateOfBirth.CalculateAge()))
                .Concat(allUserMeetings.Where(x => x.OrganizerId != userId).Select(x => x.Organizer.DateOfBirth.CalculateAge()))
                .Average();

            var allParticipants = allUserMeetings
                .SelectMany(a => a.MeetingParticipants.Where(x => x.InvitationStatus == InvitationStatus.Accepted && x.ParticipantId != userId).Select(x => x.ParticipantId))
                .Concat(allUserMeetings.Where(x => x.OrganizerId != userId).Select(x => x.OrganizerId))
                .ToList();

            var favoriteParticipantGroup = allParticipants
                .GroupBy(x => x)
                .Select(x => new
                {
                    Id = x.Key,
                    Count = x.Count()
                })
                .MaxBy(x => x.Count);


            var favParticipantUser = await dbContext
                .Users
                .Include(x => x.Image)
                .FirstOrDefaultAsync(x => x.Id == favoriteParticipantGroup.Id, cancellationToken);

            var favParticipantIdentity = mapper.Map<UserIdentityDto>(favParticipantUser);

            favoriteParticipant = new FavoriteParticipantDto()
            {
                UserIdentityDto = favParticipantIdentity,
                Count = favoriteParticipantGroup.Count
            };
            
        }

        var statisticsDto = new StatisticsDto()
        {
            MeetingsOrganized = meetingsOrganizedCount,
            MeetingsParticipated = meetingsParticipatedCount,
            TotalMinutesInMeetings = totalMinutesInMeetings,
            FavoriteSportDiscipline = favoriteSportDiscipline,
            AvgParticipantsAge = avgParticipantsAge,
            FavoriteParticipant = favoriteParticipant
        };

        return statisticsDto;
    }
}
