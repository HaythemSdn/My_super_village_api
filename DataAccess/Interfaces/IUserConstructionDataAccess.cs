using Common.Dao;

namespace DataAccess.Interfaces;

public interface IUserConstructionDataAccess
{
    Task<UserConstructionDAO?> GetActiveConstructionByUserId(Guid userId);
    Task<UserConstructionDAO> CreateConstruction(UserConstructionDAO construction);
    Task UpdateConstruction(UserConstructionDAO construction);
    Task DeleteConstruction(Guid constructionId);
}