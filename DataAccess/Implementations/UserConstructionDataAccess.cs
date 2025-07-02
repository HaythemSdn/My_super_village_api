using Common.Dao;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Implementations;

public class UserConstructionDataAccess : IUserConstructionDataAccess
{
    private readonly MyDbContext _context;

    public UserConstructionDataAccess(MyDbContext context)
    {
        _context = context;
    }

    public async Task<UserConstructionDAO?> GetActiveConstructionByUserId(Guid userId)
    {
        return await _context.UserConstructions
            .Include(c => c.Building)
            .Where(c => c.UserId == userId && !c.IsCompleted)
            .FirstOrDefaultAsync();
    }

    public async Task<UserConstructionDAO> CreateConstruction(UserConstructionDAO construction)
    {
        _context.UserConstructions.Add(construction);
        await _context.SaveChangesAsync();
        return construction;
    }

    public async Task UpdateConstruction(UserConstructionDAO construction)
    {
        _context.UserConstructions.Update(construction);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteConstruction(Guid constructionId)
    {
        var construction = await _context.UserConstructions.FindAsync(constructionId);
        if (construction != null)
        {
            _context.UserConstructions.Remove(construction);
            await _context.SaveChangesAsync();
        }
    }
}