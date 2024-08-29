using backend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace backend.Daos;

public class MongoUserDao : IUserDao
{
    private readonly IMongoCollection<User> _userCollection;
    public MongoUserDao(MongoClient client, string databaseName) {
        var mongoDatabase = client.GetDatabase(databaseName);

        _userCollection = mongoDatabase.GetCollection<User>("users");
    }
    public async Task<List<User>> GetUsers() =>
        await _userCollection.Find(_ => true).ToListAsync();

    public async Task<User?> GetUser(string username) =>
        await _userCollection.Find(x => x.Username == username).FirstOrDefaultAsync();

    public async Task CreateUser(User newUser) =>
        await _userCollection.InsertOneAsync(newUser);

    public async Task DeleteUser(string username) =>
        await _userCollection.DeleteOneAsync(x => x.Username == username);
}