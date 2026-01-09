using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Application.Dtos
{
    public class UserToolDto : IDto
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public int ToolId { get; set; }

        // aletin adını buraya ekliyoruz,react tarafında id yerine isim görelim
        public string? ToolName { get; set; }
    }
}
