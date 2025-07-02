using Common.Dao;

namespace DataAccess.Interfaces;

public interface IUserRessourceDataAccess
{
    Task<List<UserResourceDAO>> GetResourcesByUserId(Guid userId);
    Task UpdateResource(UserResourceDAO resource);
}