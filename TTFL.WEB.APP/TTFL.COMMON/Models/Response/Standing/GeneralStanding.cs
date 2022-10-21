namespace TTFL.COMMON.Models.Response.Standing
{
    public class GeneralStanding
    {
        public int PickId { get; set; }
        public string PickDate { get; set; }
        public decimal AvgPick { get; set; }
        public List<GeneralStandingResult> Results { get; set; }
    }

    public class GeneralStandingResult
    {
        public int Rank { get; set; }
        public string? Username { get; set; }
        public string? Team { get; set; }
        public int TotalPoints { get; set; }
        public decimal AvgPoints { get; set; }
    }
}
