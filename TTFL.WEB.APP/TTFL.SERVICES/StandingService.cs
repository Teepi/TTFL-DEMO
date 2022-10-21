
using Microsoft.EntityFrameworkCore;

using System.Data;
using System.Globalization;
using TTFL.COMMON.Const;
using TTFL.COMMON.Helpers.FormatHelpers;
using TTFL.COMMON.Models.Response.Standing;
using TTFL.ENTITIES;
using TTFL.SERVICES.CONTRACT;

namespace TTFL.SERVICES
{
    public class StandingService : IStandingService
    {
        private readonly TTFLContext _context;
        private readonly IPlayerService _playerService;
        private readonly IPickService _pickService;

        public StandingService(TTFLContext context,
            IPlayerService playerService,
            IPickService pickService)
        {
            _context = context;
            if (AppConsts.TTFLContext != null)
            {
                _context = AppConsts.TTFLContext;
            }

            _context.ChangeTracker.LazyLoadingEnabled = false;
            _playerService = playerService;
            _pickService = pickService;

        }

        /// <summary>
        /// Get number of best pick standing
        /// </summary>
        /// <returns></returns>
        public async Task<List<BestPickStanding>> GetBestPickStanding()
        {
            List<int> playersIds = await _playerService.GetPlayersIdsAsync();

            return await _context.PickPoints
                .Include(pp => pp.Player)
                .ThenInclude(p => p.Team)
                .Where(pp => pp.BestPick && playersIds.Contains(pp.PlayerId))
                .GroupBy(s => s.PlayerId)
                .Select(s => new BestPickStanding
                {
                    Banana = s.Select(s => s.Player.PUsername).First(),
                    Team = s.Select(s => s.Player.Team.TName).First(),
                    Count = s.Count()
                })
                .OrderByDescending(s => s.Count)
                .ToListAsync();
        }

        /// <summary>
        /// Get hell pick standing
        /// </summary>
        /// <returns></returns>
        public async Task<List<HellPickStanding>> GetHellPickStanding()
        {
            List<int> playersIds = await _playerService.GetPlayersIdsAsync();

            return await _context.PickPoints
                .Include(pp => pp.Player)
                .ThenInclude(p => p.Team)
                .Where(pp => pp.HellPick && playersIds.Contains(pp.PlayerId))
                .GroupBy(s => s.PlayerId)
                .Select(s => new HellPickStanding
                {
                    Banana = s.Select(s => s.Player.PUsername).First(),
                    Team = s.Select(s => s.Player.Team.TName).First(),
                    Count = s.Count()
                })
                .OrderByDescending(s => s.Count)
                .ToListAsync();
        }

        /// <summary>
        /// Get General Standing
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<GeneralStanding> GetGeneralStandingAsync(int? pickId)
        {
            GeneralStanding result = new();

            Pick? pick = null;
            if (pickId.HasValue)
            {
                pick = await _context.Pick.Where(p => p.PNumber == pickId).FirstAsync();
                result.PickDate = pick.PDate.Date.ToShortDateString();
                result.PickId = pick.PNumber;
            }
            else
            {
                pick = await _pickService.GetLastPickIdAsync();
                result.PickDate = pick.PDate.Date.ToShortDateString();
                result.PickId = pick.PNumber;
            }


            result.Results = await _context.PickPoints
               .Where(p => p.PickId == pick.PId)
               .OrderByDescending(p => p.TotalPoints)
               .Select(pp => new GeneralStandingResult
               {
                   Rank = pp.Rank,
                   Team = pp.Player.Team.TName.Split(" ", StringSplitOptions.None).Last(),
                   TotalPoints = pp.TotalPoints,
                   Username = pp.Player.PUsername,
                   AvgPoints = DecimalHelper.ConvertToDecimalwithDigits((decimal)pp.TotalPoints / (decimal)pp.Pick.PNumber, 2),
               }).ToListAsync();

            result.AvgPick = DecimalHelper.ConvertToDecimalwithDigits(result.Results.Select(r => r.AvgPoints).Average(), 2);

            return result;
        }

        /// <summary>
        /// Get general week standing
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<GeneralWeekStanding> GetGeneralWeekStandingAsync(string date)
        {
            GeneralWeekStanding result = new();
            CultureInfo? myCult = new("fr-FR", false);

            DateTime monday = !string.IsNullOrEmpty(date)
                ? DateTime.Parse(date, myCult)
                : await _context.Pick.MaxAsync(p => p.PDate);

            Pick? lastPick = await _context.Pick
                .FirstOrDefaultAsync(p => p.PDate.Date == DateTime.Parse(monday.Date.ToString("yyyy/MM/dd"), new CultureInfo("en-US")));

            DateTime sunday = DateTime.Now;
            if (lastPick != null)
            {
                if ((string.IsNullOrEmpty(date) && monday.DayOfWeek != DayOfWeek.Monday))
                {
                    monday = monday.AddDays(-1);
                }

                while (monday.DayOfWeek != DayOfWeek.Monday)
                {
                    monday = monday.AddDays(-1);
                }

                sunday = monday.AddDays(6);
            }
            else
            {
                sunday = DateTime.Parse(date, myCult);

                while (sunday.DayOfWeek != DayOfWeek.Sunday)
                {
                    sunday = sunday.AddDays(1);
                }

                monday = sunday.AddDays(-6);
            }

            int countPick = await _context.Pick
                .Where(p => p.PDate.Date >= monday.Date
                && p.PDate.Date <= sunday.Date)
                .CountAsync();

            if (countPick > 0)
            {
                List<KeyValuePair<int, int>>? picks = await _context.PickPoints.
                Where(p => p.Pick.PDate.Date >= monday.Date && p.Pick.PDate.Date <= sunday.Date)
               .OrderByDescending(p => p.TotalPoints)
               .GroupBy(b => b.PlayerId)
               .Select(x => new KeyValuePair<int, int>(x.Key, x.Where(x => x.PlayerId == x.PlayerId).Sum(g => g.PickerPlayerPoints)))
               .ToListAsync();

                result.Results = picks.Select(pp => new GeneralWeekStandingResult
                {
                    Team = _context.Player.Where(p => p.PId == pp.Key).Select(s => s.Team.TName.Split(" ", StringSplitOptions.None).Last()).First(),
                    TotalPoints = pp.Value,
                    Username = _context.Player.Where(p => p.PId == pp.Key).Select(s => s.PUsername).FirstOrDefault(),
                    AvgPoints = DecimalHelper.ConvertToDecimalwithDigits((decimal)pp.Value / (decimal)countPick, 2)
                })
                .OrderByDescending(x => x.AvgPoints)
                .ToList();


                result.AvgPick = DecimalHelper.ConvertToDecimalwithDigits(result.Results.Select(r => r.AvgPoints).Average(), 2);
                result.PickEndDate = sunday.ToShortDateString();
                result.PickStartDate = monday.ToShortDateString();
            }
            return result;
        }

        /// <summary>
        /// Get no Pick standing
        /// </summary>
        /// <returns></returns>
        public async Task<List<NoPickStanding>> GetNoPickStanding()
        {
            List<int> playersIds = await _playerService.GetPlayersIdsAsync();

            return await _context.PickPoints
                .Include(pp => pp.Player)
                .ThenInclude(p => p.Team)
                .Where(pp => pp.PickerPlayerPoints == 0 && playersIds.Contains(pp.PlayerId) && string.IsNullOrEmpty(pp.PickedPlayerName))
                .GroupBy(s => s.PlayerId)
                .Select(s => new NoPickStanding
                {
                    Banana = s.Select(s => s.Player.PUsername).First(),
                    Team = s.Select(s => s.Player.Team.TName).First(),
                    Count = s.Count()
                })
                .OrderByDescending(s => s.Count)
                .ToListAsync();
        }

        /// <summary>
        /// Get month standing
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<GeneralMonthStanding> GetGeneralMonthStandingAsync(int? monthId)
        {
            GeneralMonthStanding result = new();

            Pick? pick = monthId.HasValue
                ? await _context.Pick.Where(p => p.PDate.Month == monthId).OrderBy(p => p.PId).FirstAsync()
                : await _pickService.GetLastPickIdAsync();

            DateTime firstDayOfMonth = new DateTime(pick.PDate.Date.Year, pick.PDate.Date.Month, 1).Date;
            DateTime lastDayOfMonth = new DateTime(pick.PDate.Date.Year, pick.PDate.Date.Month, 1).Date.AddMonths(1).AddMilliseconds(-1);

            int countPick = _context.Pick
                .Where(p => p.PDate.Date >= firstDayOfMonth && p.PDate.Date <= lastDayOfMonth)
                .Count();

            if (countPick > 0)
            {
                List<KeyValuePair<int, int>>? picks = await _context.PickPoints.
                Where(p => p.Pick.PDate.Date >= firstDayOfMonth && p.Pick.PDate.Date <= lastDayOfMonth)
               .OrderByDescending(p => p.TotalPoints)
               .GroupBy(b => b.PlayerId)
               .Select(x => new KeyValuePair<int, int>(x.Key, x.Where(x => x.PlayerId == x.PlayerId).Sum(g => g.PickerPlayerPoints)))
               .ToListAsync();

                result.Results = picks.Select(pp => new GeneralMonthStandingResult
                {
                    Team = _context.Player.Where(p => p.PId == pp.Key).Select(s => s.Team.TName.Split(" ", StringSplitOptions.None).Last()).FirstOrDefault(),
                    TotalPoints = pp.Value,
                    Username = _context.Player.Where(p => p.PId == pp.Key).Select(s => s.PUsername).FirstOrDefault(),
                    AvgPoints = DecimalHelper.ConvertToDecimalwithDigits((decimal)pp.Value / (decimal)countPick, 2)
                })
                .OrderByDescending(x => x.AvgPoints)
                .ToList();

                result.AvgPick = DecimalHelper.ConvertToDecimalwithDigits(result.Results.Select(r => r.AvgPoints).Average(), 2);
                result.PickEndDate = lastDayOfMonth.ToShortDateString();
                result.PickStartDate = firstDayOfMonth.ToShortDateString();
                result.PickDateTime = firstDayOfMonth;
            }
            return result;
        }
    }
}
