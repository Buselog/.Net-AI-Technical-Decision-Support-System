using AutoMapper;
using RepairGuidance.Application.Dtos;
using RepairGuidance.Application.Managers;
using RepairGuidance.Contract.Repositories;
using RepairGuidance.Domain.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.InnerInfrastructure.Managers
{
    public abstract class BaseManager<D,T> : IBaseManager<D,T>  where D : class, IEntity where T : class, IDto
    {

        protected readonly IBaseRepository<D> _repository;
        protected readonly IMapper _mapper;

        public BaseManager(IBaseRepository<D> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<T>> GetAllAsync()
        {
            List<D> values = await _repository.GetAllAsync();
            return _mapper.Map<List<T>>(values);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            D value = await _repository.GetByIdAsync(id);
            return _mapper.Map<T>(value);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<D, bool>> exp)
        {
            D value = await _repository.FirstOrDefaultAsync(exp);
            return _mapper.Map<T>(value);
        }

        public async Task<bool> AnyAsync(Expression<Func<D, bool>> exp)
        {
            return await _repository.AnyAsync(exp);
        }

        public IQueryable<T> Where(Expression<Func<D, bool>> exp)
        {
            IQueryable<D> values = _repository.Where(exp);
            return _mapper.Map<IQueryable<T>>(values);
        }

        // BaseManager'dan miras alacak child manager'lar bu metodu override edebilir(ezebilir) kendine göre -> virtual
        public virtual async Task<string> AddAsync(T entity)
        {
            D domainEntity = _mapper.Map<D>(entity);
            await _repository.AddAsync(domainEntity);
            await _repository.SaveChangesAsync();
            return "Ekleme başarılı.";
        }

        public async Task<string> UpdateAsync(T entity)
        {
            D domainEntity = _mapper.Map<D>(entity);
            _repository.Update(domainEntity);
            await _repository.SaveChangesAsync();
            return "Güncelleme başarılı.";
        }


        public async Task<string> DeleteAsync(T entity)
        {
            D domain = _mapper.Map<D>(entity);
            _repository.Delete(domain);
            await _repository.SaveChangesAsync();
            return "Silme işlemi başarılı";
        }


    }
}
