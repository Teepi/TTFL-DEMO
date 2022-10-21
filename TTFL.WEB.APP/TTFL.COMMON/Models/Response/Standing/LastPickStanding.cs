namespace TTFL.COMMON.Models.Response.Standing
{
    public class LastPickStanding
    {
        public int PickId { get; set; }
        public string? PickDate { get; set; }
        public LastPickTeamResult BananaGuys { get; set; }
        public LastPickTeamResult BananaKids { get; set; }
    }

    public class LastPickTeamResult
    {
        public int TeamRank { get; set; }
        public int TeamTotalPoints { get; set; }
        public int TeamPickPoints { get; set; }
        public decimal TeamAvgPoints { get; set; }
        public int TeamEvolution { get; set; }
        public List<LastPickTeamResultDetails> TeamResultDetails { get; set; }
    }

    public class LastPickTeamResultDetails
    {
        public int Rank { get; set; }
        public int Evolution { get; set; }
        public string Banana { get; set; }
        public int TotalPoints { get; set; }
        public string LastPick { get; set; }
        public decimal AvgPick { get; set; }
        public int PickPoints { get; set; }
        public bool IsBestPick { get; set; }
        public string LastPickTeamLogo { get; set; }
    }
}
