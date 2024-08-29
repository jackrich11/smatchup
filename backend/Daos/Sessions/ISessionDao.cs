using backend.Models;

namespace backend.Daos.Sessions;

public interface ISessionDao
{
    public Task DeleteSession(string sessionId);

    public Task<Session?> GetSession(string sessionId);

    public Task<IList<Session>> GetSessionsByUser(string userId);

    public Task DeleteAllUserSessions(string userId);
    
    public  Task SetSession(Session session);
}