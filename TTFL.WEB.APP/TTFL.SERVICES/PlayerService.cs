using Microsoft.EntityFrameworkCore;
using TTFL.COMMON.Const;
using TTFL.ENTITIES;
using TTFL.SERVICES.CONTRACT;

namespace TTFL.SERVICES
{
    public class PlayerService : IPlayerService
    {
        private readonly TTFLContext _context;

        public PlayerService(TTFLContext context)
        {
            _context = context;
            if (AppConsts.TTFLContext != null)
            {
                _context = AppConsts.TTFLContext;
            }
        }

        /// <summary>
        /// Get All Players
        /// </summary>
        /// <returns></returns>
        public async Task<List<KeyValuePair<int, string>>> GetAllPlayersAsync(bool includePlayerWithoutTeam)
        {
            return await _context.Player
                .Where(p => !includePlayerWithoutTeam ? p.TeamId != null : (p.TeamId == null && p.TeamId != null))
                .OrderBy(p => p.PUsername.ToLower())
                .Select(s => new KeyValuePair<int, string>(s.PId, s.PUsername))
                .ToListAsync();
        }

        /// <summary>
        /// Get player ids
        /// </summary>
        /// <returns></returns>
        public async Task<List<int>> GetPlayersIdsAsync()
        {
            return await _context.Player
                .Where(p => p.TeamId != null)
                .Select(s => s.PId)
                .ToListAsync();
        }
    }
}
