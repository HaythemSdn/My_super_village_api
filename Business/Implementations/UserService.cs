using Business.Interfaces;
using Microsoft.Extensions.Logging;
using Common.Request;
using DataAccess.Interfaces;

namespace Business.Implementations;

public class UserService:IUserService
{
    private readonly IUsersDataAccess _userDataAccess;
    private readonly ILogger<UserService> _logger;
}