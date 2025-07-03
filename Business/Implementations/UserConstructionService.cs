using Business.Exceptions;
using Business.Interfaces;
using Common.Dao;
using Common.Dto;
using Common.Enums;
using DataAccess.Interfaces;
using Microsoft.Extensions.Logging;

namespace Business.Implementations;

public class UserConstructionService : IUserConstructionService
{
    private readonly IUserConstructionDataAccess _constructionDataAccess;
    private readonly IUserBuildingDataAccess _buildingDataAccess;
    private readonly IUserRessourceDataAccess _resourceDataAccess;
    private readonly ILogger<UserConstructionService> _logger;

    private const int BaseConstructionTimeSeconds = 5;

    public UserConstructionService(
        IUserConstructionDataAccess constructionDataAccess,
        IUserBuildingDataAccess buildingDataAccess,
        IUserRessourceDataAccess resourceDataAccess,
        ILogger<UserConstructionService> logger)
    {
        _constructionDataAccess = constructionDataAccess;
        _buildingDataAccess = buildingDataAccess;
        _resourceDataAccess = resourceDataAccess;
        _logger = logger;
    }

    public async Task<ConstructionStatusDTO> GetConstructionStatus(Guid userId)
    {
        var activeConstruction = await _constructionDataAccess.GetActiveConstructionByUserId(userId);
        
        return new ConstructionStatusDTO
        {
            ActiveConstruction = activeConstruction?.ToDto()
        };
    }

    public async Task<UpgradeBuildingResponseDTO> StartBuildingUpgrade(Guid buildingId, Guid userId)
    {
        try
        {
            // Vérifier s'il y a déjà une construction en cours
            var existingConstruction = await _constructionDataAccess.GetActiveConstructionByUserId(userId);
            if (existingConstruction != null)
            {
                throw new BusinessRuleException("Une construction est déjà en cours pour cet utilisateur");
            }

            // Récupérer le bâtiment
            var building = await _buildingDataAccess.GetBuildingById(buildingId);
            if (building == null || building.UserId != userId)
            {
                throw new BusinessRuleException("Bâtiment non trouvé ou n'appartient pas à cet utilisateur");
            }

            // Vérifier les ressources nécessaires
            await ValidateAndConsumeResources(userId, building);

            // Calculer le temps de construction (augmente avec le niveau du bâtiment ACTUEL + 1)
            var constructionDuration = BaseConstructionTimeSeconds * (building.Level + 1);
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddSeconds(constructionDuration);

            // Créer la construction
            var construction = new UserConstructionDAO
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BuildingId = buildingId,
                StartTime = startTime,
                EndTime = endTime,
                IsCompleted = false
            };

            await _constructionDataAccess.CreateConstruction(construction);

            return new UpgradeBuildingResponseDTO
            {
                Success = true,
                ConstructionEndTime = endTime.ToString("O"), // ISO 8601 format
                ConstructionDurationSeconds = constructionDuration
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du démarrage de l'amélioration du bâtiment {BuildingId}", buildingId);
            return new UpgradeBuildingResponseDTO
            {
                Success = false,
                ConstructionDurationSeconds = 0
            };
        }
    }

    public async Task<CompleteConstructionResponseDTO> CompleteConstruction(Guid buildingId, Guid userId)
    {
        try
        {
            // Récupérer la construction active
            var construction = await _constructionDataAccess.GetActiveConstructionByUserId(userId);
            if (construction == null || construction.BuildingId != buildingId)
            {
                throw new BusinessRuleException("Aucune construction active trouvée pour ce bâtiment");
            }

            // Vérifier que la construction est terminée
            if (DateTime.UtcNow < construction.EndTime)
            {
                throw new BusinessRuleException("La construction n'est pas encore terminée");
            }

            // Récupérer et mettre à jour le bâtiment
            var building = await _buildingDataAccess.GetBuildingById(buildingId);
            if (building == null)
            {
                throw new BusinessRuleException("Bâtiment non trouvé");
            }

            building.Level++;
            await _buildingDataAccess.UpdateBuilding(building);

            // Marquer la construction comme terminée et la supprimer
            await _constructionDataAccess.DeleteConstruction(construction.Id);

            return new CompleteConstructionResponseDTO
            {
                Success = true,
                NewLevel = building.Level
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la finalisation de la construction {BuildingId}", buildingId);
            return new CompleteConstructionResponseDTO
            {
                Success = false,
                NewLevel = 0
            };
        }
    }

    private async Task ValidateAndConsumeResources(Guid userId, UserBuildingDAO building)
    {
        var userResources = await _resourceDataAccess.GetResourcesByUserId(userId);
        
        var costBois = (int)Math.Pow(building.Level + 1, 2) * 50;   
        var costFer = (int)Math.Pow(building.Level + 1, 2) * 30;    
        var costPierre = (int)Math.Pow(building.Level + 1, 2) * 40; 

        var boisResource = userResources.FirstOrDefault(r => r.Type == ResourceType.Bois);
        var ferResource = userResources.FirstOrDefault(r => r.Type == ResourceType.Fer);
        var pierreResource = userResources.FirstOrDefault(r => r.Type == ResourceType.Pierre);

        if (boisResource == null || boisResource.Quantity < costBois)
            throw new BusinessRuleException($"Pas assez de bois. Requis: {costBois}, Disponible: {boisResource?.Quantity ?? 0}");
        
        if (ferResource == null || ferResource.Quantity < costFer)
            throw new BusinessRuleException($"Pas assez de fer. Requis: {costFer}, Disponible: {ferResource?.Quantity ?? 0}");
        
        if (pierreResource == null || pierreResource.Quantity < costPierre)
            throw new BusinessRuleException($"Pas assez de pierre. Requis: {costPierre}, Disponible: {pierreResource?.Quantity ?? 0}");

        boisResource.Quantity -= costBois;
        ferResource.Quantity -= costFer;
        pierreResource.Quantity -= costPierre;

        building.UpgradeCostBois = (int)Math.Pow(building.Level + 2, 2) * 50;  
        building.UpgradeCostFer = (int)Math.Pow(building.Level + 2, 2) * 30;
        building.UpgradeCostPierre = (int)Math.Pow(building.Level + 2, 2) * 40;

        await _resourceDataAccess.UpdateResource(boisResource);
        await _resourceDataAccess.UpdateResource(ferResource);
        await _resourceDataAccess.UpdateResource(pierreResource);
    }
}