namespace Common.Dao;

public class UserDAO
{
    public Guid Id { get; set; }
    public string Pseudo { get; set; } = null!;
    
    public ICollection<UserBuildingDAO> Buildings { get; set; } = new List<UserBuildingDAO>();
    public ICollection<UserResourceDAO> Resources { get; set; } = new List<UserResourceDAO>();
}