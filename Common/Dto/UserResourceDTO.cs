using Common.Dao;

namespace Common.Dto;

public class UserResourceDTO
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!; 
    public int Quantity { get; set; }
    
}

public static class UserResourcesDtoExtensions
{
    public static UserResourceDTO ToDto(this UserResourceDAO userResourceDAO)
    {
        return new UserResourceDTO
        {
            Id = userResourceDAO.Id,
            Type = userResourceDAO.Type.ToString(),
            Quantity = userResourceDAO.Quantity, 
        };
    }
}
