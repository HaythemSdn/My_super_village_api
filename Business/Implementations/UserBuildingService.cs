using Business.Interfaces;
using Common.Dto;
using DataAccess.Interfaces;

namespace Business.Implementations;

public class UserBuildingService : IUserBuildingService
{
    private readonly IUserBuildingDataAccess _userBuildingDataAccess;

    public UserBuildingService(IUserBuildingDataAccess userBuildingDataAccess)
    {
        _userBuildingDataAccess = userBuildingDataAccess;
    }

    public async Task<List<UserBuildingDTO>> GetBuildingsByUserId(Guid userId)
    {
        var daos = await _userBuildingDataAccess.GetBuildingsByUserId(userId);
        return daos.Select(b => b.ToDto()).ToList();
    }
}