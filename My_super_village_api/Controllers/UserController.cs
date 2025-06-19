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

    public UsersController(IUserService UserService) {
        _usersService = UserService;
    }
    [HttpPost]
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
}
