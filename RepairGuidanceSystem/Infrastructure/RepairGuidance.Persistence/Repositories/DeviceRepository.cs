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

        public async Task<Device?> FindBestMatchAsync(string deviceName)
        {
            if (string.IsNullOrWhiteSpace(deviceName)) return null;

            // 1. ADIM: Tam Eşleşme (Performans için önce buna bakıyoruz)
            var directMatch = await _context.Devices
                .FirstOrDefaultAsync(x => x.Name.ToLower() == deviceName.ToLower());

            if (directMatch != null) return directMatch;

            // 2. ADIM: Bulanık Arama (Pattern'i dışarıda hazırlıyoruz)
            var searchPattern = $"%{deviceName}%";

            // EF Core'un rahat çevirebileceği basit bir sorgu:
            return await _context.Devices
                .FirstOrDefaultAsync(x => EF.Functions.ILike(x.Name, searchPattern)
                                       || deviceName.ToLower().Contains(x.Name.ToLower()));
        }
    }
}
