using Common.Dto;

namespace Business.Interfaces;

public interface IUserRessourceService
{
    Task<List<UserResourceDTO>> GetResourcesByUserId(Guid userId);
}