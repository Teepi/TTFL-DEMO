namespace TTFL.COMMON.Models.Response.Standing
{
    public class GeneralMonthStanding
    {
        public string? PickStartDate { get; set; }
        public string? PickEndDate { get; set; }
        public DateTime PickDateTime { get; set; }
        public decimal AvgPick { get; set; }
        public List<GeneralMonthStandingResult>? Results { get; set; }
    }

    public class GeneralMonthStandingResult
    {
        public string? Username { get; set; }
        public string? Team { get; set; }
        public int TotalPoints { get; set; }
        public decimal AvgPoints { get; set; }
    }
}
