using RepairGuidance.Application.Dtos;
using RepairGuidance.Domain.Entities.Concretes;

namespace RepairGuidance.Application.Managers
{
    public interface IRepairStepManager : IBaseManager<RepairStep, RepairStepDto>
    {
        Task<bool> UpdateStepStatusAsync(int stepId, bool isCompleted);

        Task<RepairStepDto> GetStepByIdAsync(int id);
    }
}
