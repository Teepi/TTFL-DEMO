using Microsoft.EntityFrameworkCore;
using TTFL.COMMON.Const;
using TTFL.COMMON.Models.Response.NbaTeam;
using TTFL.ENTITIES;
using TTFL.SERVICES.CONTRACT;

namespace TTFL.SERVICES
{
    public class NbaTeamService : INbaTeamService
    {
        private readonly TTFLContext _context;

        public NbaTeamService(TTFLContext context)
        {
            _context = context;
            if (AppConsts.TTFLContext != null)
            {
                _context = AppConsts.TTFLContext;
            }
        }

        public NbaTeamService()
        {
            _context = AppConsts.TTFLContext;
        }

        /// <summary>
        /// Get all nba teams
        /// </summary>
        /// <returns></returns>
        public async Task<List<NbaTeamOutput>> GetAllNbaTeamsAsync(bool withBananaNoPick)
        {
            return await _context.NbaTeams
                .Where(x => withBananaNoPick ? x.TeamId > 0 : x.TeamId > 1)
                .Select(x => new NbaTeamOutput
                {
                    Id = x.TeamId,
                    Logo = x.Logo,
                    TeamName = x.TeamName,
                    TeamSlug = x.TeamSlug
                })
            .ToListAsync();
        }
    }
}
