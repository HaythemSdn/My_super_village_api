using Common.Dao;

namespace Common.Dto;

public class UserBuildingDTO
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!; 
    public int Level { get; set; }
    public int UpgradeCostBois { get; set; }
    public int UpgradeCostFer { get; set; }
    public int UpgradeCostPierre { get; set; }
}

public static class UserBuildingsDTOExtensions
{
    public static UserBuildingDTO ToDto(this UserBuildingDAO userBuildingDAO)
    {
        return new UserBuildingDTO
        {
            Id = userBuildingDAO.Id,
            Type = userBuildingDAO.Type.ToString(),
            Level = userBuildingDAO.Level,
            UpgradeCostBois = userBuildingDAO.UpgradeCostBois,
            UpgradeCostFer = userBuildingDAO.UpgradeCostFer,
            UpgradeCostPierre = userBuildingDAO.UpgradeCostPierre
        };
    }
}