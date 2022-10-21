namespace TTFL.COMMON.Models.Response.Standing
{
    public class HistoryResult
    {
        public HistoryResultPlayer Player { get; set; }
        public List<HistoryResultPicks> Picks { get; set; }
    }

    public class HistoryResultPicks
    {
        public int PickId { get; set; }
        public int PickNumber { get; set; }
        public string? PickDate { get; set; }
        public int Rank { get; set; }
        public int Evolution { get; set; }
        public int TotalPoints { get; set; }
        public string? PickedPlayerName { get; set; }
        public int PickerPlayerPoints { get; set; }
        public bool BestPick { get; set; }
        public int TeamPosition { get; set; }
        public bool HellPick { get; set; }
        public int PickPlayerId { get; set; }
        public string? PickPlayerTeamName { get; set; }
        public string? PickPlayerTeamLogo { get; set; }
        public decimal AvgPoints { get; set; }
        public string LastPick { get; set; }
        public string PickDateUs { get; set; }
        public bool IsPositiveEvolution { get; set; }
    }

    public class HistoryResultPlayer
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int TeamId { get; set; }
        public string? TeamName { get; set; }
    }
}
