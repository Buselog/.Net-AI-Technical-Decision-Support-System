using RepairGuidance.Contract.Repositories;
using RepairGuidance.Domain.Entities.Concretes;
using RepairGuidance.Persistence.Context;

namespace RepairGuidance.Persistence.Repositories
{
    public class SupportMessageRepository : BaseRepository<SupportMessage>, ISupportMessageRepository
    {
        public SupportMessageRepository(AppDbContext context) : base(context) {
        
        }
    }
}
