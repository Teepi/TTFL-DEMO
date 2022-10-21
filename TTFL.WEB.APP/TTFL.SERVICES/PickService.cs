using Microsoft.EntityFrameworkCore;

using TTFL.COMMON.Const;
using TTFL.COMMON.Helpers.FormatHelpers;
using TTFL.COMMON.Models.Response.Standing;
using TTFL.ENTITIES;
using TTFL.SERVICES.CONTRACT;

namespace TTFL.SERVICES
{
    public class PickService : IPickService
    {
        private readonly TTFLContext _context;

        public PickService(TTFLContext context)
        {
            _context = context;
            if (AppConsts.TTFLContext != null)
            {
                _context = AppConsts.TTFLContext;
            }
        }

        /// <summary>
        /// Get Last pick id
        /// </summary>
        /// <returns></returns>
        public async Task<Pick> GetLastPickIdAsync()
        {
            return await _context.Pick
                .OrderByDescending(p => p.PId)
                .FirstAsync();
        }


        /// <summary>
        /// Get pick History for player
        /// </summary>
        /// <returns></returns>
        public async Task<HistoryResult> GetPickHistoryAsync(int? playerId)
        {
            return new()
            {
                Player = await _context.Player
                    .Where(p => p.PId == playerId)
                    .Select(s => new HistoryResultPlayer
                    {
                        Id = s.PId,
                        Name = s.PUsername,
                        TeamId = s.TeamId.Value,
                        TeamName = s.Team.TName.Split(' ', StringSplitOptions.None).Last()
                    })
                    .FirstAsync(),
                Picks = await _context.PickPoints
                .Where(pp => pp.PlayerId == playerId)
                .Include(s => s.NbaPlayer)
                .ThenInclude(s => s.NbaTeam)
                .OrderBy(p => p.PickId)
                .Select(s => new HistoryResultPicks
                {
                    BestPick = s.BestPick,
                    Evolution = s.Evolution,
                    HellPick = s.HellPick,
                    LastPick = $"{s.NbaPlayer.PlayerFullName} ({s.PickerPlayerPoints}pts)",
                    PickedPlayerName = s.NbaPlayer.PlayerFullName,
                    PickerPlayerPoints = s.PickerPlayerPoints,
                    PickDate = s.Pick.PDate.ToShortDateString(),
                    PickId = s.PickId,
                    PickNumber = s.Pick.PNumber,
                    PickPlayerId = s.NbaPlayerId.Value,
                    PickPlayerTeamLogo = s.NbaPlayer.NbaTeam.Logo,
                    PickPlayerTeamName = s.NbaPlayer.NbaTeam.TeamName,
                    Rank = s.Rank,
                    TeamPosition = s.TeamPosition,
                    TotalPoints = s.TotalPoints,
                    AvgPoints = DecimalHelper.ConvertToDecimalwithDigits((decimal)s.TotalPoints / s.Pick.PNumber, 2),
                    IsPositiveEvolution = s.Evolution >= 0
                })
                .ToListAsync()
            };
        }

        /// <summary>
        /// Get Last Pick standing
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<LastPickStanding> GetLastPickAsync(int? pickId)
        {
            LastPickStanding result = new();

            Pick? pick = null;
            if (pickId.HasValue)
            {
                pick = await _context.Pick.Where(p => p.PNumber == pickId).FirstAsync();
                result.PickDate = pick.PDate.Date.ToShortDateString();
                result.PickId = pick.PNumber;
            }
            else
            {
                pick = await GetLastPickIdAsync();
                result.PickDate = pick.PDate.Date.ToShortDateString();
                result.PickId = pick.PNumber;
            }

            //GUYS
            List<PickPoints>? bananaGuys = await _context.PickPoints
                .Where(p => p.PickId == pick.PId && p.Player.TeamId == 4)
                .Include(n => n.NbaPlayer)
                .ThenInclude(n => n.NbaTeam)
                .OrderByDescending(pp => pp.TotalPoints)
                .ToListAsync();

            TeamRank? a = await _context.TeamRank
                .Where(tr => tr.PickId == pick.PId && tr.TeamId == 4).FirstAsync();

            int teamRankGuy = await _context.TeamRank
                .Where(tr => tr.PickId == pick.PId && tr.TeamId == 4)
                .Select(s => s.Rank).FirstAsync();

            result.BananaGuys = new LastPickTeamResult
            {
                TeamRank = teamRankGuy,
                TeamResultDetails = new List<LastPickTeamResultDetails>()
            };

            foreach (PickPoints? guy in bananaGuys)
            {
                string? playerTeamLogo = await _context.NbaPlayers
                    .Where(p => p.PlayerFullName == guy.PickedPlayerName)
                    .Select(p => p.NbaTeam.Logo)
                    .FirstOrDefaultAsync();

                Player? player = await _context.Player.Where(p => p.PId == guy.PlayerId).FirstAsync();
                result.BananaGuys.TeamResultDetails.Add(new LastPickTeamResultDetails
                {
                    Banana = player.PUsername,
                    Evolution = guy.Evolution,
                    AvgPick = DecimalHelper.ConvertToDecimalwithDigits((decimal)guy.TotalPoints / (decimal)guy.Pick.PNumber, 2),
                    LastPick = $"{guy.NbaPlayer.PlayerFullName} ({guy.PickerPlayerPoints}pts)",
                    LastPickTeamLogo = guy.NbaPlayer.NbaTeam.Logo,
                    Rank = guy.Rank,
                    TotalPoints = guy.TotalPoints,
                    PickPoints = guy.PickerPlayerPoints,
                    IsBestPick = guy.BestPick
                });
            }

            result.BananaGuys.TeamTotalPoints = result.BananaGuys.TeamResultDetails.Sum(s => s.TotalPoints);
            result.BananaGuys.TeamPickPoints = result.BananaGuys.TeamResultDetails.Sum(s => s.PickPoints);
            result.BananaGuys.TeamAvgPoints = DecimalHelper.ConvertToDecimalwithDigits((decimal)result.BananaGuys.TeamTotalPoints / (decimal)10 / (decimal)pick.PNumber, 2);

            Pick? beforeLastPick = await _context.Pick
                .Where(p => p.PNumber == pick.PNumber - 1)
                .FirstOrDefaultAsync();

            if (beforeLastPick != null)
            {
                int beforeLastTeamRankingGuys = await _context.TeamRank
                .Where(tr => tr.PickId == beforeLastPick.PId && tr.TeamId == 4)
                .Select(s => s.Rank)
                .FirstAsync();

                result.BananaGuys.TeamEvolution = beforeLastTeamRankingGuys - result.BananaGuys.TeamRank;
            }
            else
            {
                result.BananaGuys.TeamEvolution = 0;
            }

            //KIDS
            List<PickPoints>? bananaKids = await _context.PickPoints
                .Where(p => p.PickId == pick.PId && p.Player.TeamId == 5)
                .Include(n => n.NbaPlayer)
                .ThenInclude(n => n.NbaTeam)
                .OrderByDescending(pp => pp.TotalPoints)
                .ToListAsync();

            int teamRankKids = await _context.TeamRank
               .Where(tr => tr.TeamId == 5 && tr.PickId == pick.PId)
               .Select(s => s.Rank).FirstAsync();

            result.BananaKids = new LastPickTeamResult
            {
                TeamRank = teamRankKids,
                TeamResultDetails = new List<LastPickTeamResultDetails>()
            };
            foreach (PickPoints? kid in bananaKids)
            {
                Player? player = await _context.Player.Where(p => p.PId == kid.PlayerId).FirstAsync();
                result.BananaKids.TeamResultDetails.Add(new LastPickTeamResultDetails
                {
                    Banana = kid.Player.PUsername,
                    Evolution = kid.Evolution,
                    AvgPick = DecimalHelper.ConvertToDecimalwithDigits((decimal)kid.TotalPoints / (decimal)kid.Pick.PNumber, 2),
                    LastPick = $"{kid.NbaPlayer.PlayerFullName} ({kid.PickerPlayerPoints}pts)",
                    LastPickTeamLogo = kid.NbaPlayer.NbaTeam.Logo,
                    Rank = kid.Rank,
                    TotalPoints = kid.TotalPoints,
                    PickPoints = kid.PickerPlayerPoints,
                    IsBestPick = kid.BestPick
                });
            }

            result.BananaKids.TeamTotalPoints = result.BananaKids.TeamResultDetails.Sum(s => s.TotalPoints);
            result.BananaKids.TeamPickPoints = result.BananaKids.TeamResultDetails.Sum(s => s.PickPoints);
            result.BananaKids.TeamAvgPoints = DecimalHelper.ConvertToDecimalwithDigits((decimal)result.BananaKids.TeamTotalPoints / (decimal)10 / (decimal)pick.PNumber, 2);

            if (beforeLastPick != null)
            {
                int beforeLastTeamRankingKids = await _context.TeamRank
                .Where(tr => tr.PickId == beforeLastPick.PId && tr.TeamId == 5)
                .Select(s => s.Rank)
                .FirstOrDefaultAsync();

                result.BananaKids.TeamEvolution = beforeLastTeamRankingKids - result.BananaKids.TeamRank;
            }
            else
            {
                result.BananaKids.TeamEvolution = 0;
            }

            return result;
        }
    }
}
