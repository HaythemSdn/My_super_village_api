using Business.Exceptions;
using Business.Interfaces;
using Common.Dto;
using Common.Request;
using Microsoft.AspNetCore.Mvc;

namespace My_super_village_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserBuildingsController : ControllerBase
{
    private readonly IUserBuildingService _buildingService;
    private readonly IUserConstructionService _constructionService;

    public UserBuildingsController(IUserBuildingService buildingService, IUserConstructionService constructionService)
    {
        _buildingService = buildingService;
        _constructionService = constructionService;
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(List<UserBuildingDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<UserBuildingDTO>>> GetByUserId(Guid userId)
    {
        var buildings = await _buildingService.GetBuildingsByUserId(userId);
        if (buildings == null || !buildings.Any())
            return NotFound("Aucun bâtiment trouvé pour cet utilisateur");

        return Ok(buildings);
    }

    [HttpPut("{buildingId}/upgrade")]
    [ProducesResponseType(typeof(UpgradeBuildingResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UpgradeBuildingResponseDTO>> UpgradeBuilding(
        Guid buildingId, 
        [FromBody] UpgradeBuildingRequest request)
    {
        try
        {
            var result = await _constructionService.StartBuildingUpgrade(buildingId, request.UserId);
            
            if (!result.Success)
            {
                return BadRequest("Impossible de démarrer l'amélioration du bâtiment");
            }

            return Ok(result);
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    [HttpPost("{buildingId}/complete-construction")]
    [ProducesResponseType(typeof(CompleteConstructionResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CompleteConstructionResponseDTO>> CompleteConstruction(
        Guid buildingId,
        [FromBody] CompleteConstructionRequest request)
    {
        try
        {
            var result = await _constructionService.CompleteConstruction(buildingId, request.UserId);
            
            if (!result.Success)
            {
                return BadRequest("Impossible de finaliser la construction");
            }

            return Ok(result);
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Erreur interne du serveur");
        }
    }
}