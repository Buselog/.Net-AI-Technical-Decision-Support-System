using RepairGuidance.Domain.Entities.Concretes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Contract.Repositories
{
    public interface IDeviceRepository : IBaseRepository<Device>
    {
        // AI "Bu cihaz uygundur" dediğinde, bu cihazı veritabanına asenkron bir şekilde kaydedip sistemin öğrenmesini sağlamak:
        Task<Device> CreateAndReturnDeviceAsync(string name, int difficulty, int categoryId);
    }
}
