using Microsoft.Extensions.DependencyInjection;
using RepairGuidance.Application.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Application.DependencyResolvers
{
    public static class MapperServiceInjection
    {
        public static void AddMapperService(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
        }
    }
}
