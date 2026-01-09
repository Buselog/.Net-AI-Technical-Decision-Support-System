using RepairGuidance.Application.Dtos;
using RepairGuidance.Domain.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Application.Managers
{
    public interface IBaseManager<D, T> where D: class, IEntity where T : class, IDto
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);

        Task<T> FirstOrDefaultAsync(Expression<Func<D, bool>> exp);
        Task<bool> AnyAsync(Expression<Func<D, bool>> exp);

        IQueryable<T> Where(Expression<Func<D, bool>> exp);
        Task<string> AddAsync(T dto);
        Task<string> UpdateAsync(T dto);
        Task<string> DeleteAsync(T dto);

       // Soft delete(pasife çekme)
        //Task<string> RemoveAsync(T dto);
    }
}
