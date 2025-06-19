using Business.Exceptions;
using Business.Interfaces;
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
}
