using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RepairGuidance.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Persistence.DataSeeding
{
    public static class DbInitializer
    {
        public static IHost SeedDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<AppDbContext>();
                var logger = services.GetRequiredService<ILogger<AppDbContext>>();

                try
                {
                    context.Database.Migrate();
                    DataGenerator.Seed(context);
                    logger.LogInformation("Veritabanı başarıyla tohumlandı.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Veritabanı tohumlanırken bir hata oluştu.");
                }
            }
            return host;
        }
    }
}
