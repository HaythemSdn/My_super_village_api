using Common.Dao;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Implementations;

public class UserRessourceDataAccess : IUserRessourceDataAccess
{
    private readonly MyDbContext _context;

    public UserRessourceDataAccess(MyDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserResourceDAO>> GetResourcesByUserId(Guid userId)
    {
        return await _context.UserResources
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }
}