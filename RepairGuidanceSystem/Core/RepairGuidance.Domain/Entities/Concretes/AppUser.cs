using RepairGuidance.Domain.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Domain.Entities.Concretes
{
    public class AppUser : BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public int ExperienceScore { get; set; }
        public string? ExperienceLevel { get; set; }

        //Bir kullanıcının birden fazla tamir talebi ve aleti olabilir.
        public ICollection<RepairRequest> RepairRequests { get; set; }
        public ICollection<UserTool> UserTools { get; set; }


    }
}
