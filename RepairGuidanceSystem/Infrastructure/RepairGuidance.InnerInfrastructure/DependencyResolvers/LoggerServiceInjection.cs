using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace RepairGuidance.InnerInfrastructure.DependencyResolvers
{
    public static class LoggerServiceInjection 
    {
        public static void AddLoggerService(this IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Information()
               .WriteTo.Console()
               .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
               .CreateLogger();
        }
    }
}
