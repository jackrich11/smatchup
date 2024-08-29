using backend.Models;
using backend.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace backend.Services;

public class UserService {

    private readonly IMongoCollection<User> _userCollection;
    public UserService(IOptions<DatabaseSettings> databaseSettings) {
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase("project");

        _userCollection = mongoDatabase.GetCollection<User>("users");
    }

    public async Task<List<User>> GetAsync() =>
        await _userCollection.Find(_ => true).ToListAsync();

    public async Task<User?> GetAsync(string username) =>
        await _userCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    public async Task CreateAsync(User newUser) =>
        await _userCollection.InsertOneAsync(newUser);

    public async Task DeleteAsync(string username) =>
        await _userCollection.DeleteOneAsync(x => x.Username == username);
}