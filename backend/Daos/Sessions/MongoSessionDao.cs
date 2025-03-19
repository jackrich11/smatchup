using backend.Models;
using backend.Types;
using MongoDB.Driver;

namespace backend.Daos.Sessions;

public class MongoSessionDao : ISessionDao
{
    private readonly IMongoCollection<Session> _sessionCollection;

    public MongoSessionDao(MongoClient client, string databaseName) {
        var mongoDatabase = client.GetDatabase(databaseName);
        _sessionCollection = mongoDatabase.GetCollection<Session>(Strings.SESSIONS_COLLECTION_NAME);
    }
    public async Task DeleteSession(string sessionId)
    {
        await _sessionCollection.DeleteOneAsync(sessionId);
    }

    public async Task<Session?> GetSession(string sessionId)
    {
        return await _sessionCollection.Find(x => x.SessionId == sessionId).FirstOrDefaultAsync();
    }

    public async Task<IList<Session>> GetSessionsByUser(string userId)
    {
        return await _sessionCollection.Find(x => x.UserID == userId).ToListAsync();
    }

    public async Task DeleteAllUserSessions(string userId) {
        var builder = Builders<Session>.Filter;
        var userFilter = builder.Eq(x => x.UserID, userId);
        
        await _sessionCollection.DeleteManyAsync(userFilter);
    }

    public async Task SetSession(Session session)
    {
        await _sessionCollection.InsertOneAsync(session);
    }
}