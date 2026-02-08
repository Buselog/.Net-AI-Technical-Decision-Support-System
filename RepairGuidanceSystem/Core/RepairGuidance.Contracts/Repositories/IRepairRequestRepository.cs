using RepairGuidance.Domain.Entities.Concretes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Contract.Repositories
{
    public interface IRepairRequestRepository : IBaseRepository<RepairRequest>
    {
        // ML.NET eğitimi için kullanıcı bilgileriyle (AppUser) birlikte tüm listeyi çeker.
        Task<List<RepairRequest>> GetAllWithUsersAsync();
    }
}
