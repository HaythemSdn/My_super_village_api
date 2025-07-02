using Common.Dao;

namespace Common.Dto;

public class UserConstructionDTO
{
    public Guid BuildingId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsCompleted { get; set; }
}

public class ConstructionStatusDTO
{
    public UserConstructionDTO? ActiveConstruction { get; set; }
}

public class UpgradeBuildingResponseDTO
{
    public bool Success { get; set; }
    public string? ConstructionEndTime { get; set; }
    public int ConstructionDurationSeconds { get; set; }
}

public class CompleteConstructionResponseDTO
{
    public bool Success { get; set; }
    public int NewLevel { get; set; }
}

public static class UserConstructionDTOExtensions
{
    public static UserConstructionDTO ToDto(this UserConstructionDAO dao)
    {
        return new UserConstructionDTO
        {
            BuildingId = dao.BuildingId,
            StartTime = dao.StartTime,
            EndTime = dao.EndTime,
            IsCompleted = dao.IsCompleted
        };
    }
}