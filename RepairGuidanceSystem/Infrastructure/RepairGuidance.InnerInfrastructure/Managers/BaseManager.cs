using AutoMapper;
using RepairGuidance.Application.Dtos;
using RepairGuidance.Application.Managers;
using RepairGuidance.Contract.Repositories;
using RepairGuidance.Domain.Entities.Abstracts;
using System.Linq.Expressions;


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
        public virtual async Task<T> AddAsync(T dto)
        {
            D domainEntity = _mapper.Map<D>(dto);
            await _repository.AddAsync(domainEntity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<T>(domainEntity);
        }

        public async Task UpdateAsync(T dto)
        {
            D domainEntity = _mapper.Map<D>(dto);
            _repository.Update(domainEntity);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(T dto)
        {
            D domain = _mapper.Map<D>(dto);
            _repository.Delete(domain);
            await _repository.SaveChangesAsync();
        }


    }
}
