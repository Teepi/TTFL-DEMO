namespace TTFL.SERVICES.CONTRACT
{
    public interface IPlayerService
    {
        Task<List<KeyValuePair<int, string>>> GetAllPlayersAsync(bool includePlayerWithoutTeam);
        Task<List<int>> GetPlayersIdsAsync();
    }
}
