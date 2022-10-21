namespace TTFL.COMMON.Models.Response.Standing
{
    public class GeneralWeekStanding
    {
        public string? PickStartDate { get; set; }
        public string? PickEndDate { get; set; }
        public decimal AvgPick { get; set; }
        public List<GeneralWeekStandingResult>? Results { get; set; }
    }

    public class GeneralWeekStandingResult
    {
        public string? Username { get; set; }
        public string? Team { get; set; }
        public int TotalPoints { get; set; }
        public decimal AvgPoints { get; set; }
    }
}
