using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RepairGuidance.Application.Validators;

namespace RepairGuidance.Application.DependencyResolvers
{
    public static class ValidatorServiceInjection 
    {
        public static void AddValidatorServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<UserRegisterDtoValidator>();
        }
    }
}
