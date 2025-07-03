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
    private readonly IUserBuildingService _buildingService;
    private readonly IUserRessourceService _ressourceService;
    private readonly ILogger<UserService> _logger;

    public UserService(IUsersDataAccess userDataAccess,IUserBuildingService buildingService,IUserRessourceService ressourceService, ILogger<UserService> logger )
    {
        _userDataAccess = userDataAccess;
        _buildingService = buildingService;
        _ressourceService = ressourceService;
        _logger = logger;
    }
    public async Task CreateUser(CreateUserRequest User) {
        try {
            CheckPseudoBusinessRules(User.Pseudo);
            
            // Vérifier que le pseudo n'existe pas déjà
            var existingUser = await _userDataAccess.GetUserByPseudo(User.Pseudo);
            if (existingUser != null) {
                throw new BusinessRuleException($"Un utilisateur avec le pseudo '{User.Pseudo}' existe déjà");
            }
            
            await _userDataAccess.CreateUser(new UserDAO {
                Id = Guid.NewGuid(),
                Pseudo = User.Pseudo,
                Buildings = CreateDefaultBuildings(),
                Resources = CreateDefaultResources(),
                LastUpdatedAt = DateTime.UtcNow
            });
        } catch (Exception ex) {
            _logger.LogError(ex, "Erreur lors de la creation de l'utilisateur");
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
        return new List<UserBuildingDAO>
        {
            new() { 
                Id = Guid.NewGuid(), 
                Type = BuildingType.Scierie, 
                Level = 1,
                UpgradeCostBois = 100,
                UpgradeCostFer = 60,
                UpgradeCostPierre = 80
            },
            new() { 
                Id = Guid.NewGuid(), 
                Type = BuildingType.Mine, 
                Level = 1,
                UpgradeCostBois = 100,
                UpgradeCostFer = 60,
                UpgradeCostPierre = 80
            },
            new() { 
                Id = Guid.NewGuid(), 
                Type = BuildingType.Carriere, 
                Level = 1,
                UpgradeCostBois = 100,
                UpgradeCostFer = 60,
                UpgradeCostPierre = 80
            },
            new() { 
                Id = Guid.NewGuid(), 
                Type = BuildingType.Entrepot, 
                Level = 1,
                UpgradeCostBois = 100,
                UpgradeCostFer = 60,
                UpgradeCostPierre = 80
            }
        };
    }

    private static ICollection<UserResourceDAO> CreateDefaultResources()
    {
        return new List<UserResourceDAO>
        {
            new() { Id = Guid.NewGuid(), Type = ResourceType.Bois, Quantity = 500 },
            new() { Id = Guid.NewGuid(), Type = ResourceType.Fer, Quantity = 500 },
            new() { Id = Guid.NewGuid(), Type = ResourceType.Pierre, Quantity = 500 }
        };
    }
    
    public async Task<UserWithCollectionsDTO?> LoginWithPseudo(string pseudo)
    {
        var user = await _userDataAccess.GetUserByPseudo(pseudo);
        if (user == null)
            return null;
        await _ressourceService.CalculateAndApplyPassiveGeneration(user.Id);

        var buildings = await _buildingService.GetBuildingsByUserId(user.Id);
        var resources = await _ressourceService.GetResourcesByUserId(user.Id);

        var updatedUser = await _userDataAccess.GetUserById(user.Id);

        return new UserWithCollectionsDTO
        {
            Id = user.Id,
            Pseudo = user.Pseudo,
            LastUpdatedAt = updatedUser?.LastUpdatedAt ?? user.LastUpdatedAt,
            Buildings = buildings,
            Resources = resources
        };
    }

    public async Task<List<LeaderboardEntryDTO>> GetLeaderboard()
    {
        var users = await _userDataAccess.GetAllUsers();
        
        var leaderboard = new List<LeaderboardEntryDTO>();
        
        foreach (var user in users)
        {
            var buildings = await _buildingService.GetBuildingsByUserId(user.Id);
            
            var score = buildings.Sum(b => (int)Math.Pow(b.Level, 2));
            
            leaderboard.Add(new LeaderboardEntryDTO
            {
                Id = user.Id,
                Pseudo = user.Pseudo,
                Score = score,
                LastUpdatedAt = user.LastUpdatedAt
            });
        }
        
        return leaderboard.OrderByDescending(l => l.Score).ToList();
    }
}