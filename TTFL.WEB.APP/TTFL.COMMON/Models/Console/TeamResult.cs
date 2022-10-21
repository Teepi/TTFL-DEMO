namespace TTFL.COMMON.Models.Console
{
    public class TeamResult
    {
        public string Name { get; set; }
        public int Rank { get; set; }
        public List<TeamPlayer> Players { get; set; }
    }

    public class TeamPlayer
    {
        public int Rank { get; set; }
        public string KnickName { get; set; }
        public int TotalPoints { get; set; }
        public int Evolution { get; set; }
        public string Pick { get; set; }
        public int PickPoints { get; set; }
        public bool BestPick { get; set; }
    }
}
