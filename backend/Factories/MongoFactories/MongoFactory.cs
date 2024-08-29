using backend.Daos;
using backend.Daos.Matchups;
using backend.Daos.Sessions;
using backend.Daos.Settings;
using backend.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace backend.Factories.MongoFactories;

public class MongoFactory : IFactory
{
    private readonly MongoClient _client;
    private readonly IOptions<DatabaseSettings> _databaseSettings;
    public MongoFactory(IOptions<DatabaseSettings> databaseSettings) {
        _databaseSettings = databaseSettings;
        _client = new MongoClient(_databaseSettings.Value.ConnectionString);
    }
    public IUserDao GetUserDao()
    {
        return new MongoUserDao(_client, _databaseSettings.Value.DatabaseName);
    }

    public ISessionDao GetSessionDao() {
        return new MongoSessionDao(_client, _databaseSettings.Value.DatabaseName);
    }

    public IMatchupDao GetMatchupDao()
    {
        return new MongoMatchupDao(_client, _databaseSettings.Value.DatabaseName);
    }

    public ISettingsDao GetSettingsDao() {
        return new MongoSettingsDao(_client, _databaseSettings.Value.DatabaseName);
    }
}