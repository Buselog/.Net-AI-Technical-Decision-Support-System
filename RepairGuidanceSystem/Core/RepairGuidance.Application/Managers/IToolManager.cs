using RepairGuidance.Application.Dtos;
using RepairGuidance.Domain.Entities.Concretes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Application.Managers
{
    public interface IToolManager : IBaseManager<Tool, ToolDto>
    {
    }
}
