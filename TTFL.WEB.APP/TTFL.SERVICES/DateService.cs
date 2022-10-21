
using TTFL.ENTITIES;
using TTFL.SERVICES.CONTRACT;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TTFL.COMMON.Const;
using TTFL.COMMON.Models.Response.Schedule;
using Newtonsoft.Json;
using TTFL.COMMON.Models.Common;

namespace TTFL.SERVICES
{
    public class DateService : IDateService
    {
        private readonly TTFLContext _context;

        public DateService(TTFLContext context)
        {
            _context = context;
            if (AppConsts.TTFLContext != null)
            {
                _context = AppConsts.TTFLContext;
            }
        }

        /// <summary>
        /// Get Months with picks
        /// </summary>
        /// <returns></returns>
        public async Task<List<KeyValuePair<int, string>>> GetMonthDateListAsync()
        {
            CultureInfo? myCult = new("fr-FR", false);
            return await _context.Pick
                .OrderByDescending(p => p.PDate)
                .Select(item => new KeyValuePair<int, string>(item.PDate.Month, myCult.TextInfo.ToTitleCase(new DateTime(item.PDate.Date.Year, item.PDate.Date.Month, 1).Date.ToString("MMMM", myCult))))
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        /// Get Dates with picks
        /// </summary>
        /// <returns></returns>
        public async Task<List<KeyValuePair<int, string>>> GetPicksDateListAsync()
        {
            return await _context.Pick
                .OrderByDescending(p => p.PDate)
                .Select(s => new KeyValuePair<int, string>(s.PNumber, s.PDate.Date.ToString("dd/MM/yyyy")))
                .ToListAsync();
        }

        /// <summary>
        /// Get Calendar of season
        /// </summary>
        /// <returns></returns>
        public async Task<List<ScheduleGamesOutputDate>> GetScheduledGamesAsync()
        {
            List<ScheduleGamesOutputDate> result = new();
            ScheduleResultJson? scheduleResult = JsonConvert.DeserializeObject<ScheduleResultJson>(File.ReadAllText(AppConsts.Configuration.GetSection("Files")["GAMES_FILE"]));

            List<NbaTeams> teams = await _context.NbaTeams.ToListAsync();

            result.AddRange(scheduleResult?.LeagueSchedule?.GameDates?
                .Select(g => new ScheduleGamesOutputDate
                {
                    Date = DateTime.ParseExact(g.Date.Split(" ").First(), "M/d/yyyy", new CultureInfo("en-US")).ToShortDateString(),
                    AnchorDate = new DateTimeOffset(DateTime.ParseExact(g.Date.Split(" ").First(), "M/d/yyyy", CultureInfo.InvariantCulture)).ToUnixTimeSeconds(),
                    ScheduleGamesOutputMatches = g.Games?.Select(x => new ScheduleGamesOutputMatches
                    {
                        Id = x.GameId,
                        Date = Convert.ToDateTime(x.GameDateTimeUTC).AddHours(2).ToString("HH:mm"),
                        AwayTeam = x.AwayTeam?.TeamName,
                        HomeTeam = x.HomeTeam?.TeamName,
                        AwayTeamLogo = x.AwayTeam.TeamLogo,
                        HomeTeamLogo = x.HomeTeam.TeamLogo,
                        GameUrl = x.BranchLink,
                        BroadCasters = string.Join(",", x.Broadcasters?.NationalTvBroadcasters?.Select(b => b.BroadcasterDisplay).ToList()),
                        HomeTeamPoints = x.HomeTeam.Score,
                        AwayTeamPoints = x.AwayTeam.Score,
                        HomeTeamGameWin = Convert.ToBoolean(x.HomeTeam.Wins),
                        AwayTeamGameWin = Convert.ToBoolean(x.AwayTeam.Wins)
                    })
                .ToList()
                }));

            return result;
        }


        /// <summary>
        /// Get Weeks with picks
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<KeyValuePair<string, string>>> GetWeekDateListAsync()
        {
            List<KeyValuePair<string, string>> list = new();

            _context.Pick
               .OrderByDescending(d => d.PDate)
               .ToList()
               .ForEach(item =>
               {
                   DateTime date = item.PDate.Date;
                   while (date.DayOfWeek != DayOfWeek.Monday)
                   {
                       date = date.AddDays(-1);
                   }

                   list.Add(new KeyValuePair<string, string>(date.ToShortDateString(), $"{date:dd/MM/yyyy} - {date.AddDays(6):dd/MM/yyyy}"));
               });

            return list
               .Distinct()
               .ToList();
        }
    }
}
