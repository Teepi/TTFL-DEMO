using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using TTFL.COMMON.Models.Response.Statistics;
using TTFL.SERVICES.CONTRACT;

namespace TTFL.WEB.APP.Pages
{
    public class IndexModel : PageModel
    {
        public HomeStats Result { get; set; }
        private readonly IStatsService _statService;
        public IndexModel(IStatsService statsService)
        {
            _statService = statsService;
        }

        public async Task OnGetAsync()
        {
            Result = await _statService.GetStatsAsync();
        }
    }
}