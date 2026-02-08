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
    public class RepairRequestRepository : BaseRepository<RepairRequest>, IRepairRequestRepository
    {
        private readonly AppDbContext _context;
        public RepairRequestRepository(AppDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<List<RepairRequest>> GetAllWithUsersAsync()
        {
            // .AsNoTracking(): Verileri sadece okuyacağımız için EF Core'un takip mekanizmasını kapatır, performansı artırır.
            return await _context.RepairRequests
                .Include(r => r.AppUser)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
