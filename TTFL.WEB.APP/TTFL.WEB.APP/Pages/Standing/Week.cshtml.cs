using Microsoft.AspNetCore.Mvc.RazorPages;
using TTFL.COMMON.Models.Response.Standing;
using TTFL.SERVICES.CONTRACT;

namespace TTFL.WEB.APP.Pages.Standing
{
    public class WeekModel : PageModel
    {
        public GeneralWeekStanding Result { get; set; }
        public List<KeyValuePair<string, string>> Dates { get; set; }
        public string SelectedPick { get; set; }

        private readonly IStandingService _standingService;
        private readonly IDateService _dateService;
        public WeekModel(IStandingService standingService,
            IDateService dateService)
        {
            _standingService = standingService;
            _dateService = dateService;
        }

        public async Task OnGetAsync()
        {
            Dates = await _dateService.GetWeekDateListAsync();
            SelectedPick = !string.IsNullOrEmpty(HttpContext.Request.Query["pickDate"]) ? HttpContext.Request.Query["pickDate"] : string.Empty;
            Result = await _standingService.GetGeneralWeekStandingAsync(SelectedPick);
        }
    }
}
