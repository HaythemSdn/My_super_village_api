namespace Common.Dao;

public class UserConstructionDAO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserDAO User { get; set; } = null!;
    
    public Guid BuildingId { get; set; }
    public UserBuildingDAO Building { get; set; } = null!;
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsCompleted { get; set; }
}