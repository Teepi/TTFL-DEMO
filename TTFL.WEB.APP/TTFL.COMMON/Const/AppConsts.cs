using Microsoft.Extensions.Configuration;

using TTFL.ENTITIES;

namespace TTFL.COMMON.Const
{
    public static class AppConsts
    {
        public static string DbCnx { get; set; }
        public static string NbaPlayersFile { get; set; }
        public static string NbaTeamsFile { get; set; }
        public static IConfiguration Configuration { get; set; }
        public static string SelectedSeason { get; set; }
        public static TTFLContext TTFLContext { get; set; }
        public static string GamesFile { get; set; }

        public const string NoDataMessage = "It seems that there are no more bananas in the database";
        public const string ErrorMessage = "Ouch, the bananas could not be collected...";
    }
}
