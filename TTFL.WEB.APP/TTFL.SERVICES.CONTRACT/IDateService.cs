using TTFL.COMMON.Models.Response.Schedule;

namespace TTFL.SERVICES.CONTRACT
{
    public interface IDateService
    {
        Task<List<KeyValuePair<int, string>>> GetPicksDateListAsync();
        Task<List<KeyValuePair<string, string>>> GetWeekDateListAsync();
        Task<List<KeyValuePair<int, string>>> GetMonthDateListAsync();
        Task<List<ScheduleGamesOutputDate>> GetScheduledGamesAsync();
    }
}
