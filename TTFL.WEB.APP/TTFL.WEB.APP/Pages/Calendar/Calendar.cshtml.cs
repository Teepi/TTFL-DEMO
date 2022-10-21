using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using TTFL.COMMON.Models.Response.NbaTeam;
using TTFL.COMMON.Models.Response.Schedule;
using TTFL.SERVICES.CONTRACT;

namespace TTFL.WEB.APP.Pages.Calendar
{
    public class CalendarModel : PageModel
    {
        public List<ScheduleGamesOutputDate>? Result { get; set; }
        public List<NbaTeamOutput>? NbaTeams { get; set; }

        private readonly IDateService _dateService;
        private readonly INbaTeamService _nbaTeamService;
        public CalendarModel(IDateService dateService
            , INbaTeamService nbaTeamService)
        {
            _dateService = dateService;
            _nbaTeamService = nbaTeamService;
        }
        public async Task OnGetAsync()
        {
            NbaTeams = await _nbaTeamService.GetAllNbaTeamsAsync(false);
            Result = await _dateService.GetScheduledGamesAsync();
        }
    }
}
