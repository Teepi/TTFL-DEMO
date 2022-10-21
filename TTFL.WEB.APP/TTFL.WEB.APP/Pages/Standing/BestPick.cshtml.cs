using Microsoft.AspNetCore.Mvc.RazorPages;
using TTFL.COMMON.Models.Response.Standing;
using TTFL.SERVICES.CONTRACT;

namespace TTFL.WEB.APP.Pages.Standing
{
    public class BestPickModel : PageModel
    {
        public List<BestPickStanding> Result { get; set; }

        private readonly IStandingService _standingService;
        public BestPickModel(IStandingService standingService)
        {
            _standingService = standingService;
        }

        public async Task OnGetAsync()
        {
            Result = await _standingService.GetBestPickStanding();
        }
    }
}
