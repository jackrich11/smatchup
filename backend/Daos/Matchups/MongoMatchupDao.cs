using backend.Models;
using MongoDB.Driver;

namespace backend.Daos.Matchups;

public class MongoMatchupDao : IMatchupDao
{
    private readonly IMongoCollection<Matchup> _matchupCollection;

    public MongoMatchupDao(MongoClient client, string databaseName) {
        var mongoDatabase = client.GetDatabase(databaseName);
        _matchupCollection = mongoDatabase.GetCollection<Matchup>("matchups");
    }

    public async Task AddVisitorToMatchup(string visitor, string matchupId) {
        var filter = Builders<Matchup>.Filter
            .Eq(m => m.MatchupId, matchupId);
        
        var update = Builders<Matchup>.Update
            .Set(m => m.Visitor, visitor);

        await _matchupCollection.UpdateOneAsync(filter, update);
    }

    public async Task RemoveVisitorFromMatchup(string matchupId) {
        var filter = Builders<Matchup>.Filter
            .Eq(m => m.MatchupId, matchupId);
        
        var update = Builders<Matchup>.Update
            .Set(m => m.Visitor, null);

        await _matchupCollection.UpdateOneAsync(filter, update);
    }
    public async Task DeleteMatchup(string matchupId)
    {
        await _matchupCollection.DeleteOneAsync(x => x.MatchupId == matchupId);
    }

    public async Task<List<Matchup>> GetAvailableMatchups()
    {
        return await _matchupCollection.Find(x => x.Visitor == null).ToListAsync();
    }

    public async Task<List<Matchup>> GetAllMatchups()
    {
        return await _matchupCollection.Find(_ => true).ToListAsync();
    }

    public async Task SetMatchup(Matchup matchup)
    {
        await _matchupCollection.InsertOneAsync(matchup);
    }

    public async Task<Matchup> GetMatchup(string matchupId)
    {
        return await _matchupCollection.Find(x => x.MatchupId == matchupId).FirstOrDefaultAsync();
    }

    public async Task<Matchup> GetUserMatchup(string creator)
    {
        return await _matchupCollection.Find(x => x.Creator == creator).FirstOrDefaultAsync();
    }
}