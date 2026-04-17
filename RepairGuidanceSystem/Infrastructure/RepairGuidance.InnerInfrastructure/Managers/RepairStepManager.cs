using AutoMapper;
using RepairGuidance.Application.Dtos;
using RepairGuidance.Application.Managers;
using RepairGuidance.Contract.Repositories;
using RepairGuidance.Domain.Entities.Concretes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.InnerInfrastructure.Managers
{
    public class RepairStepManager : BaseManager<RepairStep, RepairStepDto> , IRepairStepManager
    {
        IRepairStepRepository _repository;

        public RepairStepManager(IRepairStepRepository repository, IMapper mapper) : base(repository, mapper)
        {
            _repository = repository;
        }

        public async Task<bool> UpdateStepStatusAsync(int stepId, bool isCompleted)
        {
            var step = await _repository.GetByIdAsync(stepId);
            if (step == null) return false;

            step.IsCompleted = isCompleted;
            _repository.Update(step);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
