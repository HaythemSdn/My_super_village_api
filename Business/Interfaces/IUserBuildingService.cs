using Common.Dto;

namespace Business.Interfaces;

public interface IUserBuildingService
{
    Task<List<UserBuildingDTO>> GetBuildingsByUserId(Guid userId);
}
