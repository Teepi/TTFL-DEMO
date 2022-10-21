using TTFL.COMMON.Models.Response.Standing;
using TTFL.ENTITIES;

namespace TTFL.SERVICES.CONTRACT
{
    public interface IPickService
    {
        Task<LastPickStanding> GetLastPickAsync(int? pickId);
        Task<HistoryResult> GetPickHistoryAsync(int? playerId);
        Task<Pick?> GetLastPickIdAsync();
    }
}
