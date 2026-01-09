using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Application.Dtos
{
    public class AppUserDto : IDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int ExperienceScore { get; set; }
        public string? ExperienceLevel { get; set; }
    }
}
