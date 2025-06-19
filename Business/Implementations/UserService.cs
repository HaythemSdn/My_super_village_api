using Business.Exceptions;
using Business.Interfaces;
using Common.Dao;
using Common.Enums;
using Common.Request;
using Microsoft.Extensions.Logging;
using DataAccess.Interfaces;

namespace Business.Implementations;

public class UserService:IUserService
{
    private readonly IUsersDataAccess _userDataAccess;
    private readonly ILogger<UserService> _logger;

    public UserService(IUsersDataAccess userDataAccess, ILogger<UserService> logger)
    {
        _userDataAccess = userDataAccess;
        _logger = logger;
    }
    public async Task CreateUser(CreateUserRequest User) {
        try {
            CheckPseudoBusinessRules(User.Pseudo);
            await _userDataAccess.CreateUser(new UserDAO {
                Id = Guid.NewGuid(),
                Pseudo = User.Pseudo,
                Buildings = CreateDefaultBuildings(),
                Resources = CreateDefaultResources()
            });
        } catch (Exception ex) {
            _logger.LogError(ex, "Erreur lors de la creation du jeu");
            throw;
        }
    }
    private static void CheckPseudoBusinessRules(string Pseudo) {
        if (string.IsNullOrWhiteSpace(Pseudo)) {
            throw new BusinessRuleException("Le nom doit être defini !");
        }
    }
    private static ICollection<UserBuildingDAO> CreateDefaultBuildings()
    {
        return new List<UserBuildingDAO>
        {
            new() { Id = Guid.NewGuid(), Type = BuildingType.Scierie, Level = 1 },
            new() { Id = Guid.NewGuid(), Type = BuildingType.Mine, Level = 1 },
            new() { Id = Guid.NewGuid(), Type = BuildingType.Carriere, Level = 1 },
            new() { Id = Guid.NewGuid(), Type = BuildingType.Entrepot, Level = 1 }
        };
    }

    private static ICollection<UserResourceDAO> CreateDefaultResources()
    {
        return new List<UserResourceDAO>
        {
            new() { Id = Guid.NewGuid(), Type = ResourceType.Bois, Quantity = 100 },
            new() { Id = Guid.NewGuid(), Type = ResourceType.Fer, Quantity = 100 },
            new() { Id = Guid.NewGuid(), Type = ResourceType.Pierre, Quantity = 100 }
        };
    }
}