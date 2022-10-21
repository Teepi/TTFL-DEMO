using Microsoft.EntityFrameworkCore;

using TTFL.COMMON.Const;
using TTFL.COMMON.Models.Response.Statistics;
using TTFL.ENTITIES;
using TTFL.SERVICES.CONTRACT;

namespace TTFL.SERVICES
{
    public class StatsService : IStatsService
    {
        private readonly TTFLContext _context;

        public StatsService(TTFLContext context)
        {
            _context = context;
            if (AppConsts.TTFLContext != null)
            {
                _context = AppConsts.TTFLContext;
            }
            _context.ChangeTracker.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Get users Stats
        /// </summary>
        /// <returns></returns>
        public async Task<HomeStats?> GetStatsAsync()
        {
            List<Statistics>? stats = await _context.Statistics
                .ToListAsync();

            if (stats.Count == 0)
            {
                return null;
            }

            Statistics? bpPlayer = stats.First(s => s.Key == "BEST_PICK_PLAYER");
            Statistics? mostPickedPlayer = stats.First(s => s.Key == "MOST_PICKED_PLAYER");
            Statistics? mostBestPickedPlayer = stats.First(s => s.Key == "MOST_BEST_PICK_PLAYER");
            Statistics? mostHellPickedPlayer = stats.First(s => s.Key == "MOST_HELL_PICK_PLAYER");
            Statistics? mostPickedteam = stats.First(s => s.Key == "MOST_PICKED_TEAM");

            return new()
            {
                CountNoPick = Convert.ToInt32(stats.First(s => s.Key == "COUNT_NO_PICK").Value),
                CountBestPick = Convert.ToInt32(stats.First(s => s.Key == "COUNT_BEST_PICK").Value),
                CountHellPick = Convert.ToInt32(stats.First(s => s.Key == "COUNT_HELL_PICK").Value),
                PickCount = Convert.ToInt32(stats.First(s => s.Key == "COUNT_PICKS").Value),
                BestPickPlayer = new HomeStatsPick
                {
                    Date = bpPlayer.Value2 ?? string.Empty,
                    Player = bpPlayer.Value ?? string.Empty,
                    Url = bpPlayer.Value3 ?? string.Empty
                },
                MostPickedPlayer = new HomeStatsPick
                {
                    Player = mostPickedPlayer.Value ?? string.Empty,
                    Points = !string.IsNullOrEmpty(mostPickedPlayer.Value) ? Convert.ToInt32(mostPickedPlayer.Value2) : 0,
                    Url = mostPickedPlayer.Value3 ?? string.Empty
                },
                MostBestPickedPlayer = new HomeStatsPick
                {
                    Player = mostBestPickedPlayer.Value ?? string.Empty,
                    Points = !string.IsNullOrEmpty(mostBestPickedPlayer.Value) ? Convert.ToInt32(mostBestPickedPlayer.Value2) : 0,
                    Url = mostBestPickedPlayer.Value3 ?? string.Empty
                },
                MostHellPickedPlayer = new HomeStatsPick
                {
                    Player = mostHellPickedPlayer.Value ?? string.Empty,
                    Points = !string.IsNullOrEmpty(mostHellPickedPlayer.Value) ? Convert.ToInt32(mostHellPickedPlayer.Value2) : 0,
                    Url = mostHellPickedPlayer.Value3 ?? string.Empty
                },
                MostPickedteam = new HomeStatsPick
                {
                    Player = mostPickedteam.Value,
                    Points = Convert.ToInt32(mostPickedteam.Value2),
                    Url = mostPickedteam.Value3 ?? string.Empty
                }
            };
        }
    }
}
