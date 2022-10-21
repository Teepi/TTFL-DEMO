using Microsoft.AspNetCore.Mvc.RazorPages;

using TTFL.COMMON.Models.Response.Standing;
using TTFL.SERVICES.CONTRACT;

namespace TTFL.WEB.APP.Pages.Standing
{
    public class MonthModel : PageModel
    {
        public GeneralMonthStanding Result { get; set; }
        public List<KeyValuePair<int, string>> Dates { get; set; }
        public int? SelectedPick { get; set; }

        private readonly IStandingService _standingService;
        private readonly IDateService _dateService;
        public MonthModel(IStandingService standingService,
            IDateService dateService)
        {
            _standingService = standingService;
            _dateService = dateService;
        }

        public async Task OnGetAsync()
        {
            Dates = await _dateService.GetMonthDateListAsync();
            SelectedPick = !string.IsNullOrEmpty(HttpContext.Request.Query["monthId"]) ? Convert.ToInt32(HttpContext.Request.Query["monthId"]) : null;
            Result = await _standingService.GetGeneralMonthStandingAsync(SelectedPick);
        }
    }
}
