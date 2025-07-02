using Common.Dao;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Implementations;

public class UserBuildingDataAccess : IUserBuildingDataAccess
{
    private readonly MyDbContext _context;

    public UserBuildingDataAccess(MyDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserBuildingDAO>> GetBuildingsByUserId(Guid userId)
    {
        return await _context.UserBuildings
            .Where(b => b.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserBuildingDAO?> GetBuildingById(Guid buildingId)
    {
        return await _context.UserBuildings
            .FirstOrDefaultAsync(b => b.Id == buildingId);
    }

    public async Task UpdateBuilding(UserBuildingDAO building)
    {
        _context.UserBuildings.Update(building);
        await _context.SaveChangesAsync();
    }
}