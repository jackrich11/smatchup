namespace backend.Daos.Sessions;

using MongoDB.Driver;
using backend.Daos.Settings;
using backend.Models;
using backend.Types;

public class MongoSettingsDao : ISettingsDao
{
    private readonly IMongoCollection<Settings> _settingsCollection;

    public MongoSettingsDao(MongoClient client, string databaseName) {
        var mongoDatabase = client.GetDatabase(databaseName);
        _settingsCollection = mongoDatabase.GetCollection<Settings>(Strings.SETTINGS_COLLECTION_NAME);
    }

    public async Task<Settings> GetSettings(string username)
    {
        return await _settingsCollection.Find(x => x.Username == username).FirstOrDefaultAsync();
    }

    public async Task CreateSettings(Settings settings)
    {
        await _settingsCollection.InsertOneAsync(settings);
    }

    public async Task UpdateSettings(Settings newSettings)
    {
        var filter = Builders<Settings>.Filter
            .Eq(s => s.Username, newSettings.Username);
        
        var update = Builders<Settings>.Update
            .Set(s => s.Region, newSettings.Region)
            .Set(s => s.PlayerSkillLevel, newSettings.PlayerSkillLevel)
            .Set(s => s.HasDelayMod, newSettings.HasDelayMod);

        await _settingsCollection.UpdateOneAsync(filter, update);
    }
}