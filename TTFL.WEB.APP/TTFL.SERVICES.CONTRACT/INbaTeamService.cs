using TTFL.COMMON.Models.Response.NbaTeam;

namespace TTFL.SERVICES.CONTRACT
{
    public interface INbaTeamService
    {
        Task<List<NbaTeamOutput>> GetAllNbaTeamsAsync(bool withBananaNoPicks);
    }
}
