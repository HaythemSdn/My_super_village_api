using Common.Dto;
using Common.Request;

namespace Business.Interfaces;

public interface IUserService
{
    Task CreateUser(CreateUserRequest user);
    Task<List<UserDTO>> GetAllUsers();
    Task<UserDTO?> GetUserById(Guid id);
    Task<UserWithCollectionsDTO?> LoginWithPseudo(string pseudo);
    Task<List<LeaderboardEntryDTO>> GetLeaderboard();
}