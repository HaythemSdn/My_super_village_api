using Business.Interfaces;
using Common.Dto;
using Microsoft.AspNetCore.Mvc;

namespace My_super_village_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserBuildingsController : ControllerBase
{
    private readonly IUserBuildingService _buildingService;

    public UserBuildingsController(IUserBuildingService buildingService)
    {
        _buildingService = buildingService;
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
}