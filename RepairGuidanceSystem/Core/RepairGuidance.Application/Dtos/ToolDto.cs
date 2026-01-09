using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Application.Dtos
{
    public class ToolDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Category { get; set; }
    }
}
