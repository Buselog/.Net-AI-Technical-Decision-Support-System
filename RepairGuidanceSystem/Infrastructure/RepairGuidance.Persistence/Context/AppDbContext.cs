using Microsoft.EntityFrameworkCore;
using RepairGuidance.Domain.Entities.Concretes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Persistence.Context
{
    public class AppDbContext : DbContext
    {
       
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Tool> Tools { get; set; }
        public DbSet<UserTool> UserTools { get; set; }
        public DbSet<RepairRequest> RepairRequests { get; set; }
        public DbSet<RepairStep> RepairSteps { get; set; }
        public DbSet<ToolCategory> ToolCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // IEntity arayüzünden türeyen tüm sınıfların Id'sinin PK olduğunu teyit eder.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Tablo ismini DbSet'teki veya Class'taki haliyle (Büyük-Küçük harf karışık) kalmaya zorlar
                entity.SetTableName(entity.DisplayName());

                // Sütun isimlerini de Property isimleriyle (Büyük-Küçük harf karışık) kalmaya zorlar
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.Name);
                }
            }
        }
    }
}
