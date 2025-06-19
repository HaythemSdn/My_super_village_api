using Common.Request;

namespace Business.Interfaces;

public interface IUserService
{
    Task CreateUser(CreateUserRequest user);
}