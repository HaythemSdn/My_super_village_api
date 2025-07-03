using Business.Interfaces;
using Common.Dao;
using Common.Dto;
using Common.Enums;
using DataAccess.Interfaces;
using Microsoft.Extensions.Logging;

namespace Business.Implementations;

public class UserRessourceService : IUserRessourceService
{
    private readonly IUserRessourceDataAccess _userRessourceDataAccess;
    private readonly IUsersDataAccess _userDataAccess;
    private readonly IUserBuildingDataAccess _buildingDataAccess;
    private readonly ILogger<UserRessourceService> _logger;
    
    private const int BaseProductionPerSecond = 10;
    private const int BaseStoragePerLevel = 1000; 

    public UserRessourceService(
        IUserRessourceDataAccess userRessourceDataAccess,
        IUsersDataAccess userDataAccess,
        IUserBuildingDataAccess buildingDataAccess,
        ILogger<UserRessourceService> logger)
    {
        _userRessourceDataAccess = userRessourceDataAccess;
        _userDataAccess = userDataAccess;
        _buildingDataAccess = buildingDataAccess;
        _logger = logger;
    }

    public async Task<List<UserResourceDTO>> GetResourcesByUserId(Guid userId)
    {
        var daos = await _userRessourceDataAccess.GetResourcesByUserId(userId);
        return daos.Select(r => r.ToDto()).ToList();
    }

    public async Task CalculateAndApplyPassiveGeneration(Guid userId)
    {
        try
        {
          
            var user = await _userDataAccess.GetUserById(userId);
            if (user == null)
            {
                _logger.LogWarning("Utilisateur {UserId} non trouvé", userId);
                return;
            }

            var timeElapsed = DateTime.UtcNow - user.LastUpdatedAt;
            _logger.LogInformation("Calcul génération pour {UserId} - LastUpdated: {LastUpdated}, Maintenant: {Now}, Temps écoulé: {Elapsed}", 
                userId, user.LastUpdatedAt, DateTime.UtcNow, timeElapsed);
            
            var buildings = await _buildingDataAccess.GetBuildingsByUserId(userId);
            var resources = await _userRessourceDataAccess.GetResourcesByUserId(userId);

            var storageLimit = await GetStorageLimit(buildings);

            await GenerateResourceType(buildings, resources, ResourceType.Bois, BuildingType.Scierie, timeElapsed, storageLimit);
            await GenerateResourceType(buildings, resources, ResourceType.Fer, BuildingType.Mine, timeElapsed, storageLimit);
            await GenerateResourceType(buildings, resources, ResourceType.Pierre, BuildingType.Carriere, timeElapsed, storageLimit);

            user.LastUpdatedAt = DateTime.UtcNow;
            await _userDataAccess.UpdateUser(user);

            _logger.LogInformation("Génération passive appliquée pour {UserId}, temps écoulé: {Time}", userId, timeElapsed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la génération passive pour {UserId}", userId);
        }
    }

    public async Task<int> CalculateResourceGeneration(UserBuildingDAO building, TimeSpan timeElapsed)
    {
        if (!IsProductionBuilding(building.Type))
            return 0;

        var productionPerSecond = building.Level * BaseProductionPerSecond;
        var secondsElapsed = timeElapsed.TotalSeconds;
        
        var totalProduction = (int)(productionPerSecond * secondsElapsed);
        
        _logger.LogDebug("Bâtiment {Type} niveau {Level}: {Production} ressources pour {Seconds}s", 
            building.Type, building.Level, totalProduction, secondsElapsed);
        
        return totalProduction;
    }

    public async Task<int> GetStorageLimit(ICollection<UserBuildingDAO> buildings)
    {
        var entrepot = buildings.FirstOrDefault(b => b.Type == BuildingType.Entrepot);
        if (entrepot == null)
        {
            _logger.LogWarning("Aucun entrepôt trouvé, utilisation limite par défaut");
            return 1000;
        }

        var storageLimit = entrepot.Level * BaseStoragePerLevel;
        _logger.LogInformation("Limite de stockage calculée: {Limit} (entrepôt niveau {Level})", 
            storageLimit, entrepot.Level);
        
        return storageLimit;
    }

    private async Task GenerateResourceType(
        ICollection<UserBuildingDAO> buildings, 
        List<UserResourceDAO> resources, 
        ResourceType resourceType, 
        BuildingType buildingType, 
        TimeSpan timeElapsed, 
        int storageLimit)
    {
        _logger.LogInformation("Génération {ResourceType} - Bâtiment: {BuildingType}", resourceType, buildingType);
        
        var productionBuilding = buildings.FirstOrDefault(b => b.Type == buildingType);
        if (productionBuilding == null)
        {
            _logger.LogWarning("Bâtiment {BuildingType} non trouvé", buildingType);
            return;
        }

        _logger.LogInformation("Bâtiment {BuildingType} trouvé - Niveau: {Level}", buildingType, productionBuilding.Level);

        var production = await CalculateResourceGeneration(productionBuilding, timeElapsed);
        if (production <= 0)
        {
            _logger.LogInformation("Aucune production calculée pour {BuildingType}", buildingType);
            return;
        }

        _logger.LogInformation("Production calculée: {Production} {ResourceType}", production, resourceType);

        var resource = resources.FirstOrDefault(r => r.Type == resourceType);
        if (resource == null)
        {
            _logger.LogWarning("Ressource {ResourceType} non trouvée", resourceType);
            return;
        }

        _logger.LogInformation("Ressource actuelle: {Current} {ResourceType}, Production: {Production}, Limite: {Limit}", 
            resource.Quantity, resourceType, production, storageLimit);

        var newQuantity = Math.Min(resource.Quantity + production, storageLimit);
        var actualProduction = newQuantity - resource.Quantity;

        if (actualProduction > 0)
        {
            resource.Quantity = newQuantity;
            await _userRessourceDataAccess.UpdateResource(resource);
            
            _logger.LogInformation("Produit {Amount} {ResourceType} - Nouvelle quantité: {NewQuantity} (limite: {Limit})", 
                actualProduction, resourceType, newQuantity, storageLimit);
        }
        else
        {
            _logger.LogInformation("Aucune production appliquée pour {ResourceType} (limite atteinte ou production nulle)", resourceType);
        }
    }

    private static bool IsProductionBuilding(BuildingType type)
    {
        return type switch
        {
            BuildingType.Scierie => true,
            BuildingType.Mine => true,
            BuildingType.Carriere => true,
            BuildingType.Entrepot => false,
            _ => false
        };
    }
}