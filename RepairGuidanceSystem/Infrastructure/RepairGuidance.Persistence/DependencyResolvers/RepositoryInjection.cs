using Microsoft.Extensions.DependencyInjection;
using RepairGuidance.Contract.Repositories;
using RepairGuidance.Persistence.Repositories;

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
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<ISupportMessageRepository, SupportMessageRepository>();
        }
    }
}
