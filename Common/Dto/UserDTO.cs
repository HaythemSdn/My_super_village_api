using Common.Dao;

namespace Common.Dto;

public class UserDTO
{
    public Guid Id { get; set; }
    public string Pseudo { get; set; } = null!;
}

public static class UserDTOExtensions
{
    public static UserDTO ToDto(this UserDAO userDAO)
    {
        return new UserDTO{
            Id = userDAO.Id,
            Pseudo = userDAO.Pseudo,
        };
    }
    
}