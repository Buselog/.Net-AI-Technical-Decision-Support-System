using RepairGuidance.Domain.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Contract.Repositories
{
    public interface IBaseRepository<T> where T : class, IEntity
    {
        //Veritabanından veri okuyan, ekleyen ve save eden yapılar Task
        // veri işaretleme(update ve delete) ise void olmalı. Performans için.

        //Okuma işlemleri
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        // Kayıt var mı yok mu hızlıca kontrol eder
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        // Sorgu Hazırlama (Database seviyesinde filtreleme için)
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);

        //Yazma işlemleri
        Task AddAsync(T entity);

        // GÜNCELLEME VE SİLME
        // ÖNEMLİ: EF Core'da Update ve Delete metotları sadece bellek üzerindeki nesneyi "işaretler". 
        // Fiziksel olarak DB'ye gitmezler. Bu yüzden bunlar asenkron değildir ve Task dönmezler.
        void Update(T entity);
        void Delete(T entity);

        // Kaydetme işlemi Task olmalı
        // int -> veritabanında etkilenen satır sayısı döndürür. (Affected Rows)
        Task<int> SaveChangesAsync();

    }
}
