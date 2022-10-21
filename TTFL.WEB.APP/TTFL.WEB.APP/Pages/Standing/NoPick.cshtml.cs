using Microsoft.AspNetCore.Mvc.RazorPages;
using TTFL.COMMON.Models.Response.Standing;
using TTFL.SERVICES.CONTRACT;

namespace TTFL.WEB.APP.Pages.Standing
{
    public class NoPickStandingModel : PageModel
    {
        public List<NoPickStanding> Result { get; set; }

        private readonly IStandingService _standingService;
        public NoPickStandingModel(IStandingService standingService)
        {
            _standingService = standingService;
        }

        public async Task OnGetAsync()
        {
            Result = await _standingService.GetNoPickStanding();
        }
    }
}
