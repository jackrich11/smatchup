using backend.Daos;
using backend.Daos.Matchups;
using backend.Daos.Sessions;
using backend.Daos.Settings;

namespace backend.Factories;

public interface IFactory {
    public IUserDao GetUserDao();
    public ISessionDao GetSessionDao();
    public IMatchupDao GetMatchupDao();
    public ISettingsDao GetSettingsDao();
}