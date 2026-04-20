using AutoMapper;
using RepairGuidance.Application.Dtos;
using RepairGuidance.Application.Managers;
using RepairGuidance.Contract.Repositories;
using RepairGuidance.Domain.Entities.Concretes;
using RepairGuidance.Domain.Exceptions;

namespace RepairGuidance.InnerInfrastructure.Managers
{
    public class AppUserManager : BaseManager<AppUser, AppUserDto> , IAppUserManager
    {
        IAppUserRepository _repository;
        public AppUserManager(IAppUserRepository repository, IMapper mapper): base(repository, mapper)
        {
            _repository = repository;
        }

        public async Task<AppUserDto> GetUserByIdAsync(int id)
        {
            var value = await _repository.GetByIdAsync(id);
            if (value == null) throw new UserNotFoundException();
            return _mapper.Map<AppUserDto>(value);
        }
    }
}
