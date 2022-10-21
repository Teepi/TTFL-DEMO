using Microsoft.AspNetCore.Mvc.RazorPages;

using Newtonsoft.Json;

using TTFL.COMMON.Models.Response.Standing;
using TTFL.SERVICES.CONTRACT;

namespace TTFL.WEB.APP.Pages.Pick
{
    public class HistoryModel : PageModel
    {
        public HistoryResult Result { get; set; }
        public int BestPickCount { get; private set; }
        public int HellPickCount { get; private set; }
        public int NoPickCount { get; private set; }
        public decimal AvgPick { get; private set; }
        public string ChartPicksArray { get; private set; }
        public List<int> ChartPicksNumberArray { get; private set; }
        public List<KeyValuePair<int, string>> Players { get; set; }
        public int? SelectedPlayer { get; private set; }
        public string SelectedPlayerName { get; set; }

        private readonly IPickService _pickService;
        private readonly IPlayerService _playerService;
        public HistoryModel(IPickService pickService,
            IPlayerService playerService)
        {
            _pickService = pickService;
            _playerService = playerService;

        }

        public async Task OnGetAsync()
        {
            Players = await _playerService.GetAllPlayersAsync(false);
            KeyValuePair<string, string> selectedCookiePlayer = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "selectedHistoryPickPlayer");
            SelectedPlayer = !string.IsNullOrEmpty(HttpContext.Request.Query["playerId"]) ? Convert.ToInt32(HttpContext.Request.Query["playerId"]) : null;

            if (SelectedPlayer != null && selectedCookiePlayer.Key == null)
            {
                SelectedPlayer = Convert.ToInt32(HttpContext.Request.Query["playerId"]);
            }
            else if (selectedCookiePlayer.Key != null && SelectedPlayer == null)
            {
                SelectedPlayer = Convert.ToInt32(selectedCookiePlayer.Value);
            }

            if (SelectedPlayer != null)
            {
                SelectedPlayerName = Players.FirstOrDefault(s => s.Key == SelectedPlayer).Value;
                Response.Cookies.Append("selectedHistoryPickPlayer",
                                        SelectedPlayer.ToString(),
                                        new CookieOptions { Expires = DateTime.Now.AddMonths(12) });

                Result = await _pickService.GetPickHistoryAsync(SelectedPlayer);
                BestPickCount = Result.Picks.Where(p => p.BestPick).Count();
                HellPickCount = Result.Picks.Where(p => p.HellPick).Count();
                NoPickCount = Result.Picks.Where(p => p.PickedPlayerName == null).Count();
                AvgPick = Result.Picks.OrderByDescending(p => p.PickNumber).Select(s => s.AvgPoints).FirstOrDefault();
                ChartPicksArray = JsonConvert.SerializeObject(Result.Picks.Select(p => p.Rank).ToArray());
            }
        }
    }
}
