using TTFL.COMMON.Models.Response.Standing;

namespace TTFL.SERVICES.CONTRACT
{
    public interface IStandingService
    {
        Task<GeneralStanding> GetGeneralStandingAsync(int? pickId);        
        Task<List<BestPickStanding>> GetBestPickStanding();
        Task<List<HellPickStanding>> GetHellPickStanding();
        Task<List<NoPickStanding>> GetNoPickStanding();
        Task<GeneralWeekStanding> GetGeneralWeekStandingAsync(string date);
        Task<GeneralMonthStanding> GetGeneralMonthStandingAsync(int? pickId);
        
    }
}
