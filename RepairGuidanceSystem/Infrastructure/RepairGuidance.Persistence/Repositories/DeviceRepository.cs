using Microsoft.EntityFrameworkCore;
using RepairGuidance.Contract.Repositories;
using RepairGuidance.Domain.Entities.Concretes;
using RepairGuidance.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Persistence.Repositories
{
    public class DeviceRepository : BaseRepository<Device>, IDeviceRepository
    {
        public DeviceRepository(AppDbContext context) : base(context)
        {

        }
        public async Task<Device> CreateAndReturnDeviceAsync(string name, int difficulty, int categoryId)
        {
            var newDevice = new Device
            {
                Name = name,
                DifficultyScore = difficulty,
                ToolCategoryId = categoryId,
                IsPredefined = false, // Kullanıcı tarafından sisteme kazandırıldı, yani önceden tanımlı mı: false
                CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
            };

            await _context.Devices.AddAsync(newDevice);
            await SaveChangesAsync();
            return newDevice;
        }
    }
}
