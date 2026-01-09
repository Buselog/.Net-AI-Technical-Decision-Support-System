using AutoMapper;
using RepairGuidance.Application.Dtos;
using RepairGuidance.Application.Managers;
using RepairGuidance.Contract.Repositories;
using RepairGuidance.Domain.Entities.Concretes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.InnerInfrastructure.Managers
{
    public class AppUserManager : BaseManager<AppUser, AppUserDto> , IAppUserManager
    {
        IAppUserRepository _repository;
        public AppUserManager(IAppUserRepository repository, IMapper mapper): base(repository, mapper)
        {
            _repository = repository;
        }
    }
}
