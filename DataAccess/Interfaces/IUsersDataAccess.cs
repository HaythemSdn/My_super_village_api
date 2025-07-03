using Common.Dao;

namespace DataAccess.Interfaces;

public interface IUsersDataAccess
{
    Task CreateUser(UserDAO user);
    Task<List<UserDAO>> GetAllUsers();
    Task<UserDAO?> GetUserById(Guid id);
    Task<UserDAO?> GetUserByPseudo(string pseudo);
    Task UpdateUser(UserDAO user);
}