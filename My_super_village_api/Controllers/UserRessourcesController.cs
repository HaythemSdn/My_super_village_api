using Business.Interfaces;
using Common.Dto;
using Microsoft.AspNetCore.Mvc;

namespace My_super_village_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserRessourcesController : ControllerBase
{
    private readonly IUserRessourceService _ressourceService;
    public UserRessourcesController(IUserRessourceService ressourceService)
    {
        _ressourceService = ressourceService;
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(List<UserResourceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<UserResourceDTO>>> GetByUserId(Guid userId)
    {
        var ressources = await _ressourceService.GetResourcesByUserId(userId);
        if (ressources == null || !ressources.Any())
            return NotFound("Aucune ressource trouvée pour cet utilisateur");

        return Ok(ressources);
    }
}