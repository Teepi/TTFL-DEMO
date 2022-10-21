namespace TTFL.COMMON.Models.Response.Statistics
{
    public class HomeStats
    {
        public int CountNoPick { get; set; }
        public int CountBestPick { get; set; }
        public int CountHellPick { get; set; }
        public HomeStatsPick BestPickPlayer { get; set; }
        public int PickCount { get; set; }
        public HomeStatsPick MostPickedPlayer { get; set; }
        public HomeStatsPick MostBestPickedPlayer { get; set; }
        public HomeStatsPick MostHellPickedPlayer { get; set; }
        public HomeStatsPick MostPickedteam { get; set; }
    }

    public class PickedPlayer
    {
        public string Player { get; set; }
        public int Count { get; set; }
    }

    public class HomeStatsPick
    {
        public string Player { get; set; }
        public int Points { get; set; }
        public string Url { get; set; }
        public string Date { get; set; }
    }
}
