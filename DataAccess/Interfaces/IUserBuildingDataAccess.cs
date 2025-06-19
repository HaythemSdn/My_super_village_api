using Common.Dao;

namespace DataAccess.Interfaces;

public interface IUserBuildingDataAccess
{
    Task<List<UserBuildingDAO>> GetBuildingsByUserId(Guid userId);
}