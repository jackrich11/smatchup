using System.Data.SqlTypes;
using backend.Models;

namespace backend.Daos.Matchups;

public interface IMatchupDao {
    public Task SetMatchup(Matchup matchup);
    public Task AddVisitorToMatchup(string visitor, string matchupId);
    public  Task RemoveVisitorFromMatchup(string matchupId);
    public Task<Matchup> GetMatchup(string matchupId);
    public Task<List<Matchup>> GetAvailableMatchups();
    public Task<List<Matchup>> GetAllMatchups();
    public Task DeleteMatchup(string owner);

    public Task<Matchup> GetUserMatchup(string creator);

    
}