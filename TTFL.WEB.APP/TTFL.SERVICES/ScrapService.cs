using TTFL.COMMON.Const;
using TTFL.COMMON.Models.Console;
using TTFL.ENTITIES;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TTFL.Services
{
    public class ScrapService
    {
        private readonly TTFLContext _context;
        public ScrapService()
        {
            _context = AppConsts.TTFLContext;
        }

        /// <summary>
        /// Insert data to database
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public async Task<KeyValuePair<int?, bool>> InsertTeamDataAsync(TeamResult team)
        {
            Team? teamResult = await _context.Team.FirstAsync(t => t.TName == team.Name);
            Pick? pick = await _context.Pick.FirstAsync(p => p.PDate.Date == DateTime.Now.Date.AddDays(-1));

            if (pick == null)
            {
                pick = new Pick
                {
                    PDate = DateTime.Now.Date.AddDays(-1),
                    PNumber = _context.Pick.Max(p => p.PNumber) + 1,
                    PIsScrapped = false
                };
                await _context.Pick.AddAsync(pick);
            }

            List<PickPoints>? ppToDelete = await _context.PickPoints
                .Where(pp => pp.PickId == pick.PId && pp.Player.TeamId == teamResult.TId)
                .ToListAsync();
            if (ppToDelete != null)
                _context.PickPoints.RemoveRange(ppToDelete);

            PickTeamJson? ptjToDelete = await _context.PickTeamJson
                .FirstOrDefaultAsync(ptj => ptj.PickId == pick.PId && ptj.TeamId == teamResult.TId);
            if (ptjToDelete != null)
                _context.PickTeamJson.Remove(ptjToDelete);

            TeamRank? trToDelete = await _context.TeamRank
                .FirstOrDefaultAsync(tr => tr.PickId == pick.PId && tr.TeamId == teamResult.TId);
            if (trToDelete != null)
                _context.TeamRank.Remove(trToDelete);

            _context.PickTeamJson.Add(new PickTeamJson
            {
                PickId = pick.PId,
                TeamId = teamResult.TId,
                Json = JsonConvert.SerializeObject(team.Players)
            });

            _context.TeamRank.Add(new TeamRank
            {
                TeamId = teamResult.TId,
                PickId = pick.PId,
                Rank = team.Rank
            });

            foreach (TeamPlayer? item in team.Players.OrderBy(p => p.Rank).ToList())
            {
                NbaPlayers? nbaPlayer = await _context.NbaPlayers.FirstOrDefaultAsync(p => p.PlayerFullName == item.Pick.Split("(", StringSplitOptions.None).First().Trim());
                _context.PickPoints.Add(new PickPoints
                {
                    Rank = item.Rank,
                    Evolution = item.Evolution,
                    TotalPoints = item.TotalPoints,
                    PickedPlayer = item.Pick,
                    PickedPlayerName = item.Pick.Split("(").First().Trim(),
                    PickerPlayerPoints = item.PickPoints,
                    BestPick = item.BestPick,
                    HellPick = item.PickPoints < 20,
                    TeamPosition = team.Players.IndexOf(item) + 1,
                    PickId = pick.PId,
                    PlayerId = _context.Player.First(p => p.PUsername == item.KnickName).PId,
                    NbaPlayerId = nbaPlayer != null ? nbaPlayer.PersonId : 1
                });
            }

            await _context.SaveChangesAsync();
            return new KeyValuePair<int?, bool>(pick.PId, true);
        }

        /// <summary>
        /// Check if daily scrap is done
        /// </summary>
        /// <returns></returns>
        public async Task<KeyValuePair<int?, bool>> CheckDailyScrapAsync()
        {
            DateTime currentDate = DateTime.Now;
            GameDate? gameDate = await _context.GameDate.FirstOrDefaultAsync(x => x.Date.Date == currentDate.Date.AddDays(-1));

            if (gameDate != null)
            {
                Pick? pick = await _context.Pick
                .FirstOrDefaultAsync(p => p.PDate.Date == DateTime.Now.Date.AddDays(-1));
                if (pick == null)
                {
                    pick = new Pick
                    {
                        PDate = DateTime.Now.Date.AddDays(-1),
                        PIsScrapped = false,
                        PNumber = await _context.Pick.MaxAsync(s => s.PNumber) + 1,
                    };
                    await _context.Pick.AddAsync(pick);
                    await _context.SaveChangesAsync();
                }

                return pick.PIsScrapped
                    ? new KeyValuePair<int?, bool>(pick.PNumber, true)
                    : new KeyValuePair<int?, bool>(pick.PNumber, false);
            }
            else
            {
                return new KeyValuePair<int?, bool>(0, true);
            }
        }

        /// <summary>
        /// Update daily scrap status
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateDailyScrap(int? pickId, bool isScrapped)
        {
            Pick? pick = await _context.Pick.FirstAsync(p => p.PId == pickId);
            pick.PIsScrapped = isScrapped;
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Update stats for picks
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateStatisticsDataAsync()
        {
            List<Statistics>? stats = await _context.Statistics.ToListAsync();
            List<PickPoints>? pickPoints = await _context.PickPoints
                .Include(p => p.NbaPlayer)
                .Include(p => p.NbaPlayer.NbaTeam)
                .ToListAsync();

            pickPoints.AddRange(await _context.PickPoints.Where(pp => string.IsNullOrEmpty(pp.PickedPlayerName)).ToListAsync());
            pickPoints = pickPoints.DistinctBy(pp => pp.PpId).ToList();

            //Most picked player
            var mostPickPlayer = pickPoints
                .Where(p => p.NbaPlayerId != 1)
                .GroupBy(pp => pp.PickedPlayerName)
                .OrderByDescending(pp => pp.Count())
                .Select(pp => new
                {
                    Player = pp.Key,
                    Count = pp.Count(),
                    Logo = pp.Select(s => s.NbaPlayer.NbaTeam.Logo).FirstOrDefault()
                })
                .FirstOrDefault();

            if (mostPickPlayer != null)
            {
                Statistics? mostPickPlayerToEdited = stats.First(s => s.Key == "MOST_PICKED_PLAYER");
                mostPickPlayerToEdited.Value = mostPickPlayer.Player;
                mostPickPlayerToEdited.Value2 = mostPickPlayer.Count.ToString();
                mostPickPlayerToEdited.Value3 = mostPickPlayer.Logo;
                _context.Entry(mostPickPlayerToEdited);
            }

            //No pick count
            int countNoPick = pickPoints.Count(pp => string.IsNullOrEmpty(pp.PickedPlayerName));
            Statistics? noPickCountToEdited = stats.First(s => s.Key == "COUNT_NO_PICK");
            noPickCountToEdited.Value = countNoPick.ToString();
            _context.Entry(noPickCountToEdited);

            //Best pick Count
            int countBestPick = pickPoints.Count(pp => pp.BestPick);
            Statistics? bestPickCountToEdited = stats.First(s => s.Key == "COUNT_BEST_PICK");
            bestPickCountToEdited.Value = countBestPick.ToString();
            _context.Entry(bestPickCountToEdited);

            //Hell pick count
            int countHellPick = pickPoints.Count(pp => pp.PickerPlayerPoints < 20);
            Statistics? hellPickCountToEdited = stats.First(s => s.Key == "COUNT_HELL_PICK");
            hellPickCountToEdited.Value = countHellPick.ToString();
            _context.Entry(hellPickCountToEdited);

            //Pick count
            int pickCount = await _context.Pick.CountAsync();
            Statistics? pickCountToEdited = stats.First(s => s.Key == "COUNT_PICKS");
            pickCountToEdited.Value = pickCount.ToString();
            _context.Entry(pickCountToEdited);

            //Best pick player
            var bestPickPlayer = pickPoints
                .OrderByDescending(pp => pp.PickerPlayerPoints)
                .Select(pp => new
                {
                    Player = pp.PickedPlayer,
                    Logo = pp.NbaPlayer.NbaTeam.Logo,
                    Date = _context.Pick.First(p => p.PId == pp.PickId).PDate.ToShortDateString()
                })
                .FirstOrDefault();

            if (bestPickPlayer != null)
            {
                Statistics? bestPickPlayerToEdited = stats.First(s => s.Key == "BEST_PICK_PLAYER");
                bestPickPlayerToEdited.Value = bestPickPlayer.Player.ToString();
                bestPickPlayerToEdited.Value2 = bestPickPlayer.Date.ToString();
                bestPickPlayerToEdited.Value3 = bestPickPlayer.Logo;
                _context.Entry(bestPickPlayerToEdited);
            }

            //Most Best Pick
            var mostBestPickPlayer = pickPoints
                .Where(pp => pp.BestPick)
                .GroupBy(pp => pp.PickedPlayerName)
                .OrderByDescending(pp => pp.Count())
                .Select(pp => new
                {
                    Player = pp.Key,
                    Count = pp.Count(),
                    Logo = pp.Select(s => s.NbaPlayer.NbaTeam.Logo).FirstOrDefault()
                })
                .FirstOrDefault();

            if (mostBestPickPlayer != null)
            {
                Statistics? mostBestPickPlayerToEdited = stats.First(s => s.Key == "MOST_BEST_PICK_PLAYER");
                mostBestPickPlayerToEdited.Value = mostBestPickPlayer.Player;
                mostBestPickPlayerToEdited.Value2 = mostBestPickPlayer.Count.ToString();
                mostBestPickPlayerToEdited.Value3 = mostBestPickPlayer.Logo;
                _context.Entry(mostBestPickPlayerToEdited);
            }

            //Most Hell pick player
            var mostHellPickPlayer = pickPoints
                .Where(pp => pp.HellPick && !string.IsNullOrEmpty(pp.PickedPlayerName))
                .GroupBy(pp => pp.PickedPlayerName)
                .OrderByDescending(pp => pp.Count())
                .Select(pp => new
                {
                    Player = pp.Key,
                    Count = pp.Count(),
                    Logo = pp.Select(s => s.NbaPlayer.NbaTeam.Logo).FirstOrDefault()
                })
                .FirstOrDefault();

            if (mostHellPickPlayer != null)
            {
                Statistics? mostHellPickPlayerToEdited = stats.First(s => s.Key == "MOST_HELL_PICK_PLAYER");
                mostHellPickPlayerToEdited.Value = mostHellPickPlayer.Player;
                mostHellPickPlayerToEdited.Value2 = mostHellPickPlayer.Count.ToString();
                mostHellPickPlayerToEdited.Value3 = mostHellPickPlayer.Logo;
                _context.Entry(mostHellPickPlayerToEdited);
            }

            //Most pickedTeam
            var mostPickTeam = pickPoints
               .GroupBy(s => s.PickedPlayerName)
               .Select(s => new
               {
                   Player = s.Select(s => s.NbaPlayer).First(),
                   Count = s.Count()
               })
               .GroupBy(s => s.Player?.TeamId)
               .Select(s => new
               {
                   Team = s.Select(s => s.Player?.NbaTeam?.TeamName).First(),
                   Count = s.Sum(t => t.Count),
                   Logo = s.Select(s => s.Player?.NbaTeam?.Logo).First()
               })
               .OrderByDescending(s => s.Count)
               .FirstOrDefault();

            if (mostPickTeam != null)
            {
                Statistics? mostPickedTeamToEdited = stats.First(s => s.Key == "MOST_PICKED_TEAM");
                mostPickedTeamToEdited.Value = mostPickTeam.Team;
                mostPickedTeamToEdited.Value2 = mostPickTeam.Count.ToString();
                mostPickedTeamToEdited.Value3 = mostPickTeam.Logo;
                _context.Entry(mostPickedTeamToEdited);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Insert or update nba players
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> InsertOrUpdateNbaPlayerAsync()
        {
            string json = await File.ReadAllTextAsync(AppConsts.NbaPlayersFile);

            foreach (NbaPlayers? player in JsonConvert.DeserializeObject<List<NbaPlayers>>(json))
            {
                NbaPlayers? ply = await _context.NbaPlayers.FirstOrDefaultAsync(p => p.PersonId == player.PersonId);
                if (ply != null)
                {
                    ply.TeamId = player.TeamId;
                    _context.Entry(ply);
                }
                else
                {
                    await _context.AddAsync(new NbaPlayers
                    {
                        PersonId = player.PersonId,
                        PlayerFirstName = player.PlayerFirstName,
                        PlayerLastName = player.PlayerLastName,
                        TeamId = player.TeamId,
                        PlayerFullName = $"{player.PlayerFirstName} {player.PlayerLastName}"
                    });
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> InsertOrUpdateNbaTeamsAsync()
        {
            string json = await File.ReadAllTextAsync(AppConsts.NbaTeamsFile);
            foreach (NbaTeams? team in JsonConvert.DeserializeObject<List<NbaTeams>>(json))
            {
                NbaTeams? tm = await _context.NbaTeams.FirstOrDefaultAsync(t => t.TeamId == team.TeamId);
                if (tm == null)
                {
                    await _context.AddAsync(new NbaTeams
                    {
                        TeamId = team.TeamId,
                        Logo = team.Logo,
                        TeamName = team.TeamName,
                    });
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Insert date to games date table
        /// </summary>
        /// <param name="dates"></param>
        /// <returns></returns>
        public async Task<bool> InsertGamesDates(List<string> dates)
        {
            _context.GameDate.RemoveRange(_context.GameDate.ToList());

            List<GameDate> datesToInsert = new();
            datesToInsert.AddRange(dates.Select(date => new GameDate
            {
                Date = Convert.ToDateTime(date)
            }));

            _context.Database.ExecuteSqlRaw("UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE NAME='GAME_DATE';");

            await _context.GameDate.AddRangeAsync(datesToInsert);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
