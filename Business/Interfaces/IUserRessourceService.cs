using Common.Dao;
using Common.Dto;

namespace Business.Interfaces;

public interface IUserRessourceService
{
    Task<List<UserResourceDTO>> GetResourcesByUserId(Guid userId);
    Task CalculateAndApplyPassiveGeneration(Guid userId);
    Task<int> CalculateResourceGeneration(UserBuildingDAO building, TimeSpan timeElapsed);
    Task<int> GetStorageLimit(ICollection<UserBuildingDAO> buildings);
}