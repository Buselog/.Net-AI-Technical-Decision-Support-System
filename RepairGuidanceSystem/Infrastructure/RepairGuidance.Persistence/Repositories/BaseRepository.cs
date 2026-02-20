using Microsoft.EntityFrameworkCore;
using RepairGuidance.Contract.Repositories;
using RepairGuidance.Domain.Entities.Abstracts;
using RepairGuidance.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Persistence.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class, IEntity
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;
        
        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync(){
           return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> exp)
        {
            return await _dbSet.FirstOrDefaultAsync(exp);
        }
          

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> exp)
        {
            return await _dbSet.AnyAsync(exp);
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> exp)
        {
            return _dbSet.Where(exp);
        }

        // --- YAZMA İŞLEMLERİ ---
        // Repository sadece işaretler, Manager kaydeder. Bu nedenle Add, Update ve Delete içlerinde SaveChanges yapmıyoruz.
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        // --- İŞARETLEME (VOID) ---
       // GÜNCELLEME VE SİLME: Sadece RAM'de işaretleme yapar, Task/Async GEREKMEZ!
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity); // Sadece State = Deleted yapar
        }

        // --- FİZİKSEL KAYIT ---
        // Yukarıdaki tüm işlemleri tek bir Transaction olarak veritabanına gönderir.
        // KAYDETME: Fiziksel işlemi yapan tek yer burası
        public async Task<int> SaveChangesAsync(){

           return await _context.SaveChangesAsync();
        }     

    }
}
