using Common.Dto;

namespace Business.Interfaces;

public interface IUserConstructionService
{
    Task<ConstructionStatusDTO> GetConstructionStatus(Guid userId);
    Task<UpgradeBuildingResponseDTO> StartBuildingUpgrade(Guid buildingId, Guid userId);
    Task<CompleteConstructionResponseDTO> CompleteConstruction(Guid buildingId, Guid userId);
}