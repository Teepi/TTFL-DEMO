using Newtonsoft.Json;

namespace TTFL.COMMON.Models.Common
{
    public class AwayTeam
    {
        public int? TeamId { get; set; }
        public string? TeamName { get; set; }
        public string? TeamCity { get; set; }
        public string? TeamTricode { get; set; }
        public string? TeamSlug { get; set; }
        public int? Wins { get; set; }
        public int? Losses { get; set; }
        public int? Score { get; set; }
        public int? Seed { get; set; }
        public string? TeamLogo { get; set; }
    }

    public class AwayTvBroadcaster
    {
        public string? BroadcasterScope { get; set; }
        public string? BroadcasterMedia { get; set; }
        public int? BroadcasterId { get; set; }
        public string? BroadcasterDisplay { get; set; }
        public string? BroadcasterAbbreviation { get; set; }
        public string? TapeDelayComments { get; set; }
        public int? RegionId { get; set; }
    }

    public class BroadcasterList
    {
        public int? BroadcasterId { get; set; }
        public string? BroadcasterDisplay { get; set; }
        public string? BroadcasterAbbreviation { get; set; }
        public int? RegionId { get; set; }
    }

    public class Broadcasters
    {
        public List<NationalTvBroadcaster>? NationalTvBroadcasters { get; set; }
        public List<NationalRadioBroadcaster>? NationalRadioBroadcasters { get; set; }
        public List<HomeTvBroadcaster>? HomeTvBroadcasters { get; set; }
        public List<object>? HomeRadioBroadcasters { get; set; }
        public List<AwayTvBroadcaster>? AwayTvBroadcasters { get; set; }
        public List<object>? AwayRadioBroadcasters { get; set; }
        public List<object>? IntlRadioBroadcasters { get; set; }
        public List<object>? IntlTvBroadcasters { get; set; }
    }

    public class Game
    {
        public string? GameId { get; set; }
        public string? GameCode { get; set; }
        public int? GameStatus { get; set; }
        public string? GameStatusText { get; set; }
        public int? GameSequence { get; set; }
        public DateTime? GameDateEst { get; set; }
        public DateTime? GameTimeEst { get; set; }
        public DateTime? GameDateTimeEst { get; set; }
        public DateTime? GameDateUTC { get; set; }
        public DateTime? GameTimeUTC { get; set; }
        public DateTime? GameDateTimeUTC { get; set; }
        public DateTime? AwayTeamTime { get; set; }
        public DateTime? HomeTeamTime { get; set; }
        public string? Day { get; set; }
        public int? MonthNum { get; set; }
        public int? WeekNumber { get; set; }
        public string? WeekName { get; set; }
        public bool? IfNecessary { get; set; }
        public string? SeriesGameNumber { get; set; }
        public string? SeriesText { get; set; }
        public string? ArenaName { get; set; }
        public string? ArenaState { get; set; }
        public string? ArenaCity { get; set; }
        public string? PostponedStatus { get; set; }
        public string? BranchLink { get; set; }
        public Broadcasters? Broadcasters { get; set; }
        public HomeTeam? HomeTeam { get; set; }
        public AwayTeam? AwayTeam { get; set; }
        public List<object>? PointsLeaders { get; set; }
    }

    public class GameDate
    {
        [JsonProperty("GameDate")]
        public string? Date { get; set; }
        public List<Game>? Games { get; set; }
    }

    public class HomeTeam
    {
        public int? TeamId { get; set; }
        public string? TeamName { get; set; }
        public string? TeamCity { get; set; }
        public string? TeamTricode { get; set; }
        public string? TeamSlug { get; set; }
        public int? Wins { get; set; }
        public int? Losses { get; set; }
        public int? Score { get; set; }
        public int? Seed { get; set; }
        public string? TeamLogo { get; set; }
    }

    public class HomeTvBroadcaster
    {
        public string? BroadcasterScope { get; set; }
        public string? BroadcasterMedia { get; set; }
        public int? BroadcasterId { get; set; }
        public string? BroadcasterDisplay { get; set; }
        public string? BroadcasterAbbreviation { get; set; }
        public string? TapeDelayComments { get; set; }
        public int? RegionId { get; set; }
    }

    public class LeagueSchedule
    {
        public string? SeasonYear { get; set; }
        public string? LeagueId { get; set; }
        public List<GameDate>? GameDates { get; set; }
        public List<object>? Weeks { get; set; }
        public List<BroadcasterList>? BroadcasterList { get; set; }
    }

    public class Meta
    {
        public int? Version { get; set; }
        public string? Request { get; set; }
        public DateTime? Time { get; set; }
    }

    public class NationalRadioBroadcaster
    {
        public string? BroadcasterScope { get; set; }
        public string? BroadcasterMedia { get; set; }
        public int? BroadcasterId { get; set; }
        public string? BroadcasterDisplay { get; set; }
        public string? BroadcasterAbbreviation { get; set; }
        public string? TapeDelayComments { get; set; }
        public int? RegionId { get; set; }
    }

    public class NationalTvBroadcaster
    {
        public string? BroadcasterScope { get; set; }
        public string? BroadcasterMedia { get; set; }
        public int? BroadcasterId { get; set; }
        public string? BroadcasterDisplay { get; set; }
        public string? BroadcasterAbbreviation { get; set; }
        public string? TapeDelayComments { get; set; }
        public int? RegionId { get; set; }
    }

    public class ScheduleResultJson
    {
        public Meta? Meta { get; set; }
        public LeagueSchedule? LeagueSchedule { get; set; }
    }
}
