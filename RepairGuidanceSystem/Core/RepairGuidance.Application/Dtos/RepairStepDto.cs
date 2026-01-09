using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Application.Dtos
{
    public class RepairStepDto : IDto
    {
        public int Id { get; set; }
        public int StepNumber { get; set; }
        public string Instruction { get; set; }
        public string? ToolSuggestion { get; set; } 
        public bool IsCompleted { get; set; }
    }
}

