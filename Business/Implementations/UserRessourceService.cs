using Business.Interfaces;
using Common.Dto;
using DataAccess.Interfaces;

namespace Business.Implementations;

public class UserRessourceService : IUserRessourceService
{
    private readonly IUserRessourceDataAccess _userRessourceDataAccess;

    public UserRessourceService(IUserRessourceDataAccess userRessourceDataAccess)
    {
        _userRessourceDataAccess = userRessourceDataAccess;
    }

    public async Task<List<UserResourceDTO>> GetResourcesByUserId(Guid userId)
    {
        var daos = await _userRessourceDataAccess.GetResourcesByUserId(userId);
        return daos.Select(r => r.ToDto()).ToList();
    }
}