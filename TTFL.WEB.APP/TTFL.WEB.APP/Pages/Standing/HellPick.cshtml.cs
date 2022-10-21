using Microsoft.AspNetCore.Mvc.RazorPages;
using TTFL.COMMON.Models.Response.Standing;
using TTFL.SERVICES.CONTRACT;

namespace TTFL.WEB.APP.Pages.Standing
{
    public class HellPickModel : PageModel
    {
        public List<HellPickStanding> Result { get; set; }

        private readonly IStandingService _standingService;
        public HellPickModel(IStandingService standingService)
        {
            _standingService = standingService;
        }

        public async Task OnGetAsync()
        {
            Result = await _standingService.GetHellPickStanding();
        }
    }
}
