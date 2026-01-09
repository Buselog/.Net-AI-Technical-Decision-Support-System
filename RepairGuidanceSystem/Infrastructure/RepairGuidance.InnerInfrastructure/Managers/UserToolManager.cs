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
    public class UserToolManager : BaseManager<UserTool, UserToolDto>, IUserToolManager
    {
        IUserToolRepository _repository;
        public UserToolManager(IUserToolRepository repository, IMapper mapper) : base(repository, mapper)
        {
            _repository = repository;
        }

        // BaseManager'daki AddAsync'i override ederek UserToolManager için davranışını değiştiriyoruz
        //Çünkü UserToolDto içindeki yeni string alan, veritabanına yeni Id'ler ekletiyor var olan bir ToolId girilmesine rağmen.
        // Biz de override ederek, navigation property'lerini null yapıyoruz.
        // Böylece EF Core sadece ToolId ve AppUserId sütunlarını doldurur, 
        // gidip Tool tablosuna yeni satır eklemeye çalışmaz.
        public override async Task<string> AddAsync(UserToolDto dto)
        {
            var entity = _mapper.Map<UserTool>(dto);

            entity.AppUser = null;
            entity.Tool = null;

           await _repository.AddAsync(entity);
           await _repository.SaveChangesAsync();

          return "Alet çantanıza başarıyla eklendi.";
        }

    }
}
