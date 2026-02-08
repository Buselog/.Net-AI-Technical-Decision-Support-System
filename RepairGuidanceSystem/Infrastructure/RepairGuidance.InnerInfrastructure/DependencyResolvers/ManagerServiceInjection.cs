using Microsoft.Extensions.DependencyInjection;
using RepairGuidance.Application.Managers;
using RepairGuidance.InnerInfrastructure.Managers;
using RepairGuidance.InnerInfrastructure.Managers.Prediction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.InnerInfrastructure.DependencyResolvers
{
    public static class ManagerServiceInjection
    {
        public static void AddManagerServices(this IServiceCollection services) {
            services.AddScoped<IAppUserManager, AppUserManager>();
            services.AddScoped<IToolManager, ToolManager>();
            services.AddScoped<IUserToolManager, UserToolManager>();
            services.AddScoped<IRepairRequestManager, RepairRequestManager>();
            services.AddScoped<IRepairStepManager, RepairStepManager>();
            services.AddScoped<IPredictionManager, PredictionManager>();

        }
    }
}
