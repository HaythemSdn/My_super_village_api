namespace Common.Dao;

public class UserDAO
{
    public Guid Id { get; set; }
    public string Pseudo { get; set; } = null!;
    
    public ICollection<UserBuildingDAO> Buildings { get; set; } = [];
    public ICollection<UserResourceDAO> Resources { get; set; } = [];
    
    public DateTime LastUpdatedAt { get; set; }
}