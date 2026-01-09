using RepairGuidance.Domain.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Domain.Entities.Concretes
{
    public class ToolCategory : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<Tool> Tools { get; set; }
    }
}

