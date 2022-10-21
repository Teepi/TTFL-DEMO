using Microsoft.AspNetCore.Mvc.RazorPages;

using TTFL.COMMON.Models.Response.Standing;
using TTFL.SERVICES.CONTRACT;

namespace TTFL.WEB.APP.Pages.Standing
{
    public class LastPickModel : PageModel
    {
        public LastPickStanding Result { get; set; }
        public List<KeyValuePair<int, string>> Dates { get; set; }
        public int? SelectedPick { get; set; }

        private readonly IPickService _pickService;
        private readonly IDateService _dateService;
        public LastPickModel(IPickService pickService,
             IDateService dateService)
        {
            _pickService = pickService;
            _dateService = dateService;
        }

        public async Task OnGetAsync()
        {
            Dates = await _dateService.GetPicksDateListAsync();
            SelectedPick = !string.IsNullOrEmpty(HttpContext.Request.Query["pickId"]) ? Convert.ToInt32(HttpContext.Request.Query["pickId"]) : null;
            Result = await _pickService.GetLastPickAsync(SelectedPick);
        }
    }
}
