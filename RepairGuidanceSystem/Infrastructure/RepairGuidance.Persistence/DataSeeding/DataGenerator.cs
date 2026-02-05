using Bogus;
using RepairGuidance.Domain.Entities.Concretes;
using RepairGuidance.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Persistence.DataSeeding
{
    public static class DataGenerator
    {
        public static void Seed(AppDbContext context)
        {
            // 1. KORUMA (GUARD CLAUSE)
            // Niçin: Eğer veritabanında zaten kullanıcı varsa işlemi durdurur. 
            // Bu, manuel girdiğin verilerin üstüne tekrar veri yazılmasını veya 
            // her açılışta veritabanının şişmesini engeller.
            if (context.AppUsers.Any()) return;

            // Niçin: Türkçe yerelleştirme ayarı. İsimlerin "John" yerine "Ahmet" 
            // gelmesini sağlayarak gerçekçi bir test ortamı sunar.
            var faker = new Faker("tr");

            // 2. KULLANICI ÜRETİMİ (AppUser)
            // Niçin: ML.NET'in "Tecrübeli kullanıcılar daha başarılı tamir yapar" 
            // örüntüsünü öğrenebilmesi için anlamlı skorlar üretiyoruz.
            var userFaker = new Faker<AppUser>("tr")
                .RuleFor(u => u.FullName, f => f.Name.FullName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.ExperienceScore, f => f.Random.Number(0, 100))
                // Niçin: Skor ve Seviye arasında mantıksal bir bağ kuruyoruz.
                .RuleFor(u => u.ExperienceLevel, (f, u) => u.ExperienceScore > 75 ? "Uzman" :
                                                          u.ExperienceScore > 40 ? "Orta" : "Acemi")
                // Niçin: PostgreSQL 'timestamp with time zone' beklediği için UTC dönüşümü şarttır.
                .RuleFor(u => u.CreatedDate, f => f.Date.Past(1).ToUniversalTime());

            var users = userFaker.Generate(50); // 50 farklı profil yeterli olacaktır.
            context.AppUsers.AddRange(users);
            context.SaveChanges(); // Talepler için User ID'lerine ihtiyacımız var.

            // 3. TAMİR TALEPLERİ (RepairRequest)
            // Niçin: ML.NET modelimizin ana eğitim setidir.
            var statusOptions = new[] { "Completed", "Failed" }; // Başarı/Başarısızlık oranı tahmin için kritiktir.
            var targetLevels = new[] { "Acemi", "Orta", "Uzman" };

            var repairRequestFaker = new Faker<RepairRequest>("tr")
                .RuleFor(r => r.DeviceName, f => f.Commerce.ProductName()) // "Akülü Matkap", "Laptop" vb.
                .RuleFor(r => r.ProblemDescription, f => f.Lorem.Sentence()) // "Cihaz tetik almıyor", "Ekran gidip geliyor"
                .RuleFor(r => r.Status, f => f.PickRandom(statusOptions))
                .RuleFor(r => r.TargetLevel, f => f.PickRandom(targetLevels))
                // Niçin: Mevcut kullanıcılardan rastgele birini bu talebe atar (İlişkisel veritabanı gereği).
                .RuleFor(r => r.AppUserId, f => f.PickRandom(users).Id)
                .RuleFor(r => r.CreatedDate, f => f.Date.Past(1).ToUniversalTime());

            var requests = repairRequestFaker.Generate(1000); // ML.NET için 1000 kayıt iyi bir başlangıçtır.
            context.RepairRequests.AddRange(requests);
            context.SaveChanges();

            // 4. TAMİR ADIMLARI (RepairStep)
            // Niçin: Her talebe ait adımları üreterek LLM'den gelen veriyi simüle ederiz.
            foreach (var request in requests)
            {
                var stepFaker = new Faker<RepairStep>("tr")
                    .RuleFor(s => s.RepairRequestId, _ => request.Id) // Talebe bağlar.
                    .RuleFor(s => s.StepNumber, (f, s) => f.IndexGlobal + 1)
                    .RuleFor(s => s.Instruction, f => f.Lorem.Sentence())
                    // Niçin: Senin Regex mantığına uygun sahte bir alet ismi üretir.
                    .RuleFor(s => s.ToolSuggestion, f => f.Commerce.Product())
                    // Niçin: Eğer talep 'Completed' ise tüm adımlarını da tamamlandı yapar.
                    .RuleFor(s => s.IsCompleted, f => request.Status == "Completed");

                context.RepairSteps.AddRange(stepFaker.Generate(faker.Random.Number(2, 5)));
            }
            context.SaveChanges();
        }

    }
}
