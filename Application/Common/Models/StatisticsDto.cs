using Application.Statistics.Queries;

namespace Application.Common.Models;

public class StatisticsDto
{
    public int MeetingsOrganized { get; set; }
    public int MeetingsParticipated { get; set; }
    public int TotalMinutesInMeetings { get; set; }
    public FavoriteDisciplineDto? FavoriteSportDiscipline { get; set; }
    public int? AvgParticipantsAge { get; set; }
    public FavoriteParticipantDto? FavoriteParticipant { get; set; }
}
