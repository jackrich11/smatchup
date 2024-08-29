using backend.Models;

namespace backend.Daos;

public interface IUserDao {
    public Task<List<User>> GetUsers();
    //public Task<User?> GetUser(string Id);

    public Task<User?> GetUser(string username);
    public Task CreateUser(User newUser);
    public Task DeleteUser(string Id);
}