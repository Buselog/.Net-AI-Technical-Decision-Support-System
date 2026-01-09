using RepairGuidance.Application.Dtos;
using RepairGuidance.Domain.Entities.Concretes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Application.Managers
{
    public interface IAppUserManager : IBaseManager<AppUser, AppUserDto>
    {
        //Crud dışında entity'ye özel metotlar gelecekse imzaları buraya eklenir.
    }
}
