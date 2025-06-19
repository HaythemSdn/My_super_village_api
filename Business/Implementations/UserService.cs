using Business.Exceptions;
using Business.Interfaces;
using Common.Dao;
using Common.Request;
using Microsoft.Extensions.Logging;
using DataAccess.Interfaces;

namespace Business.Implementations;

public class UserService:IUserService
{
    private readonly IUsersDataAccess _userDataAccess;
    private readonly ILogger<UserService> _logger;

    public UserService(IUsersDataAccess userDataAccess, ILogger<UserService> logger)
    {
        _userDataAccess = userDataAccess;
        _logger = logger;
    }
    public async Task CreateUser(CreateUserRequest User) {
        try {
            CheckPseudoBusinessRules(User.Pseudo);
            await _userDataAccess.CreateUser(new UserDAO {
                Id = Guid.NewGuid(),
                Pseudo = User.Pseudo,
            });
        } catch (Exception ex) {
            _logger.LogError(ex, "Erreur lors de la creation du jeu");
            throw;
        }
    }
    private static void CheckPseudoBusinessRules(string Pseudo) {
        if (string.IsNullOrWhiteSpace(Pseudo)) {
            throw new BusinessRuleException("Le nom doit être defini !");
        }
    }
}