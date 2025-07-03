using Common.Dao;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

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
    public async Task<List<UserDAO>> GetAllUsers()
    {
        return await _myDbContext.Users.ToListAsync();
    }

    public async Task<UserDAO?> GetUserById(Guid id)
    {
        return await _myDbContext.Users.FindAsync(id);
    }
    public async Task<UserDAO?> GetUserByPseudo(string pseudo)
    {
        return await _myDbContext.Users.FirstOrDefaultAsync(u => u.Pseudo == pseudo);
    }

    public async Task UpdateUser(UserDAO user)
    {
        _myDbContext.Users.Update(user);
        await _myDbContext.SaveChangesAsync();
    }
}