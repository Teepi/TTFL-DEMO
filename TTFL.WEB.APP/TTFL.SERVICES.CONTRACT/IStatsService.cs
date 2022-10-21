using TTFL.COMMON.Models.Response.Statistics;

namespace TTFL.SERVICES.CONTRACT
{
    public interface IStatsService
    {
        Task<HomeStats> GetStatsAsync();
    }
}
