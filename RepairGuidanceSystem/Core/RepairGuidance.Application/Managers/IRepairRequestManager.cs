using RepairGuidance.Application.Dtos;
using RepairGuidance.Domain.Entities.Concretes;

namespace RepairGuidance.Application.Managers
{
    public interface IRepairRequestManager : IBaseManager<RepairRequest, RepairRequestDto>
    {
        Task<RepairRequestDto> CreateAiSupportGuidanceAsync(CreateRepairRequestDto dto);

        Task<string> CompleteRepairRequestAsync(int requestId);

        Task<string> GetSupportForStepAsync(AiSupportRequestDto dto);
    }
}
