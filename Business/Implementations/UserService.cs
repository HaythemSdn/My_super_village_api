using Business.Exceptions;
using Business.Interfaces;
using Common.Dao;
using Common.Dto;
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
    public async Task<List<UserDTO>> GetAllUsers()
    {
        var users = await _userDataAccess.GetAllUsers();
        return users.Select(u => u.ToDto()).ToList();
    }

    public async Task<UserDTO?> GetUserById(Guid id)
    {
        var user = await _userDataAccess.GetUserById(id);
        return user?.ToDto();
    }
    private static void CheckPseudoBusinessRules(string Pseudo) {
        if (string.IsNullOrWhiteSpace(Pseudo)) {
            throw new BusinessRuleException("Le nom doit être defini !");
        }
    }
    private static ICollection<UserBuildingDAO> CreateDefaultBuildings()
    {
        var now = DateTime.UtcNow;
        return new List<UserBuildingDAO>
        {
            new() { Id = Guid.NewGuid(), Type = BuildingType.Scierie, Level = 1 ,LastUpdatedAt = now},
            new() { Id = Guid.NewGuid(), Type = BuildingType.Mine, Level = 1 ,LastUpdatedAt = now},
            new() { Id = Guid.NewGuid(), Type = BuildingType.Carriere, Level = 1 ,LastUpdatedAt = now},
            new() { Id = Guid.NewGuid(), Type = BuildingType.Entrepot, Level = 1 ,LastUpdatedAt = now}
        };
    }

    private static ICollection<UserResourceDAO> CreateDefaultResources()
    {
        var now = DateTime.UtcNow;
        return new List<UserResourceDAO>
        {
            new() { Id = Guid.NewGuid(), Type = ResourceType.Bois, Quantity = 100,LastUpdatedAt = now },
            new() { Id = Guid.NewGuid(), Type = ResourceType.Fer, Quantity = 100 ,LastUpdatedAt = now},
            new() { Id = Guid.NewGuid(), Type = ResourceType.Pierre, Quantity = 100 ,LastUpdatedAt = now}
        };
    }
}