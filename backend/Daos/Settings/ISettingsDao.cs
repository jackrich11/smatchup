namespace backend.Daos.Settings;

using backend.Models;

public interface ISettingsDao {
    public Task CreateSettings(Settings settings);
    public Task<Settings> GetSettings(string username);
    public Task UpdateSettings(Settings newSettings);

}