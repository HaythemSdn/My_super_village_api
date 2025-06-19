using Common.Dao;

namespace DataAccess.Interfaces;

public interface IUsersDataAccess
{
    Task CreateUser(UserDAO user);
}