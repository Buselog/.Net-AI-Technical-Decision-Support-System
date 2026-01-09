using RepairGuidance.Domain.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Domain.Entities.Concretes
{
    public class UserTool : BaseEntity
    {
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; } // Navigation prop.
        public int ToolId { get; set; }
        public Tool Tool { get; set; }
    }
}
