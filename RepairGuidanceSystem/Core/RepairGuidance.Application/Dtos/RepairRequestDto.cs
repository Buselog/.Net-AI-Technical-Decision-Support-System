using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Application.Dtos
{
    public class RepairRequestDto : IDto
    {
        public int Id { get; set; }
        public string? ProblemDescription { get; set; } 
        public string DeviceName { get; set; } 
        public string? TargetLevel { get; set; } 
        public string Status { get; set; } 
        public DateTime CreatedDate { get; set; }

        // ilişkili adımlar
        public List<RepairStepDto>? Steps { get; set; } 
    }
}
