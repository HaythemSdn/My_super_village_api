using Common.Enums;

namespace Common.Dao;

public class UserBuildingDAO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserDAO User { get; set; } = null!;

    public BuildingType Type { get; set; }
    public int Level { get; set; }
    public DateTime LastUpdatedAt { get; set; }

    public int UpgradeCostBois { get; set; }
    public int UpgradeCostFer { get; set; }
    public int UpgradeCostPierre { get; set; }
}