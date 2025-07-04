using Business.Exceptions;
using Business.Interfaces;
using Common.Dto;
using Common.Request;
using Microsoft.AspNetCore.Mvc;

namespace My_super_village_api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _usersService;
    private readonly IUserConstructionService _constructionService;

    public UsersController(IUserService userService, IUserConstructionService constructionService) {
        _usersService = userService;
        _constructionService = constructionService;
    }
    [HttpPost("signup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateUser([FromBody] CreateUserRequest request) {
        try {
            await _usersService.CreateUser(request);
            return Ok();
        } catch (BusinessRuleException e) {
            return BadRequest(e.Message);
        }
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserDTO>>> GetAllUsers()
    {
        var users = await _usersService.GetAllUsers();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDTO>> GetUserById(Guid id)
    {
        var user = await _usersService.GetUserById(id);
        if (user == null)
            return NotFound();
        return Ok(user);
    }
    
    [HttpGet("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LoginWithPseudo(string pseudo)
    {
        var user = await _usersService.LoginWithPseudo(pseudo);
        if (user == null)
            return NotFound($"Aucun utilisateur trouvé avec le pseudo '{pseudo}'");

        return Ok(user);
    }

    [HttpGet("{userId}/constructions")]
    [ProducesResponseType(typeof(ConstructionStatusDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<ConstructionStatusDTO>> GetConstructionStatus(Guid userId)
    {
        var status = await _constructionService.GetConstructionStatus(userId);
        return Ok(status);
    }

    [HttpGet("leaderboard")]
    [ProducesResponseType(typeof(List<LeaderboardEntryDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LeaderboardEntryDTO>>> GetLeaderboard()
    {
        var leaderboard = await _usersService.GetLeaderboard();
        return Ok(leaderboard);
    }
}
