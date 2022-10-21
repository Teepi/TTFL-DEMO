namespace TTFL.COMMON.Models.Response.Schedule
{
    public class ScheduleGamesOutputDate
    {
        public string? Date { get; set; }
        public List<ScheduleGamesOutputMatches>? ScheduleGamesOutputMatches { get; set; }
        public long AnchorDate { get; set; }
    }

    public class ScheduleGamesOutputMatches
    {
        public string? Id { get; set; }
        public string? Date { get; set; }
        public string? HomeTeam { get; set; }
        public string? AwayTeam { get; set; }
        public string? AwayTeamLogo { get; set; }
        public string? HomeTeamLogo { get; set; }
        public string GameUrl { get; set; }
        public string BroadCasters { get; set; }
        public int? HomeTeamPoints { get; set; }
        public int? AwayTeamPoints { get; set; }
        public bool HomeTeamGameWin { get; set; }
        public bool AwayTeamGameWin { get; set; }
    }
}


