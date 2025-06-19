using Common.Dao;
using DataAccess.Interfaces;
namespace DataAccess.Implementations;

public class UsersDataAccess :IUsersDataAccess
{
    private readonly MyDbContext _myDbContext;

    public UsersDataAccess(MyDbContext myDbContext)
    {
        _myDbContext = myDbContext;
    }
    public async Task CreateUser(UserDAO User) {
        _myDbContext.Users.Add(User);
        await _myDbContext.SaveChangesAsync();
    }
}