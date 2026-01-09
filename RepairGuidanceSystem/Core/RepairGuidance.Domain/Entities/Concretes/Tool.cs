using RepairGuidance.Domain.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Domain.Entities.Concretes
{
    public class Tool : BaseEntity
    {
        public string Name { get; set; }
        public int ToolCategoryId { get; set; }
        public ToolCategory ToolCategory { get; set; }

    }
}

