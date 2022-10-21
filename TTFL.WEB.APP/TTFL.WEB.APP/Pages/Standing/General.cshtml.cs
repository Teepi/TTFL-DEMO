using Microsoft.AspNetCore.Mvc.RazorPages;

using TTFL.COMMON.Models.Response.Standing;
using TTFL.SERVICES.CONTRACT;

namespace TTFL.WEB.APP.Pages.Standing
{
    public class GeneralModel : PageModel
    {
        public List<GeneralStandingResult> Result { get; set; }
        public List<KeyValuePair<int, string>> Dates { get; set; }
        public int? SelectedPick { get; set; }

        public int? PickId { get; set; }
        public decimal AvgPick { get; set; }
        public string PickDate { get; set; }

        private readonly IStandingService _standingService;
        private readonly IDateService _dateService;
        public GeneralModel(IStandingService standingService, IDateService dateService)
        {
            _standingService = standingService;
            _dateService = dateService;
        }

        public async Task OnGetAsync()
        {
            Dates = await _dateService.GetPicksDateListAsync();
            SelectedPick = !string.IsNullOrEmpty(HttpContext.Request.Query["pickId"]) ? Convert.ToInt32(HttpContext.Request.Query["pickId"]) : null;
            GeneralStanding? generalStanding = await _standingService.GetGeneralStandingAsync(SelectedPick);
            PickId = generalStanding.PickId;
            PickDate = generalStanding.PickDate;
            AvgPick = generalStanding.AvgPick;
            Result = generalStanding.Results;
        }
    }
}
