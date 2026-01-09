using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Application.Dtos
{
    public class CreateRepairRequestDto : IDto
    {
        public int AppUserId { get; set; }
        public string DeviceName { get; set; }
        public string? ProblemDescription { get; set; }
        public string? TargetLevel { get; set; }
        public List<string> AvailableTools { get; set; } 
    }
}
