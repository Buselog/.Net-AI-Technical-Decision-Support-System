using RepairGuidance.Application.Dtos;
using RepairGuidance.Domain.Entities.Abstracts;
using System.Linq.Expressions;


namespace RepairGuidance.Application.Managers
{
    public interface IBaseManager<D, T> where D: class, IEntity where T : class, IDto
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> FirstOrDefaultAsync(Expression<Func<D, bool>> exp);
        Task<bool> AnyAsync(Expression<Func<D, bool>> exp);
        IQueryable<T> Where(Expression<Func<D, bool>> exp);
        Task<T> AddAsync(T dto);
        Task UpdateAsync(T dto);
        Task DeleteAsync(T dto);
    }
}
