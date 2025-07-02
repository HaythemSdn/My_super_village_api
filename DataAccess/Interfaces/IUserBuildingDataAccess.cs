using Common.Dao;

namespace DataAccess.Interfaces;

public interface IUserBuildingDataAccess
{
    Task<List<UserBuildingDAO>> GetBuildingsByUserId(Guid userId);
    Task<UserBuildingDAO?> GetBuildingById(Guid buildingId);
    Task UpdateBuilding(UserBuildingDAO building);
}