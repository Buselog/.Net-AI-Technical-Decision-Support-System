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
    public class ToolRepository : BaseRepository<Tool>, IToolRepository
    {
        public ToolRepository(AppDbContext context) : base(context)
        {

        }
    }
}
