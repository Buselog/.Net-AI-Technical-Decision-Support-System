using Microsoft.Extensions.DependencyInjection;
using RepairGuidance.Contract.Repositories;
using RepairGuidance.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Persistence.DependencyResolvers
{
    public static class RepositoryInjection
    {
        public static void AddRepositoryServices(this IServiceCollection services)
        {
            // Repository Kayıtları (Scoped)
            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IToolRepository, ToolRepository>();
            services.AddScoped<IUserToolRepository, UserToolRepository>();
            services.AddScoped<IRepairRequestRepository, RepairRequestRepository>();
            services.AddScoped<IRepairStepRepository, RepairStepRepository>();
        }
    }
}
