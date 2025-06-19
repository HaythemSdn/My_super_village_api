using Common.Enums;

namespace Common.Dao;

public class UserResourceDAO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserDAO User { get; set; } = null!;
    
    public DateTime LastUpdatedAt { get; set; }
    public ResourceType Type { get; set; }
    public int Quantity { get; set; }
}