using RepairGuidance.Domain.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Domain.Entities.Concretes
{
    public class RepairRequest : BaseEntity
    {
        public string? ProblemDescription { get; set; }

        // Tamir edilen cihazın adı
        //Yapay Zeka'ya (LLM) daha kaliteli ve eksiksiz veri göndermektir.
        /*
         * Neden DeviceName'i bu tablodan silmiyoruz(çünkü Device tablosu var ?)
        Kullanıcı sisteme kayıtlı olmayan bir cihaz (örneğin "Antika Saat") girdiğinde, DeviceId null olacaktır. 
        Cihazın ne olduğunu bilmemiz için kullanıcının girdiği o ham metni (DeviceName) saklamamız şart.

        Arama ve Geçmiş: Kullanıcı geçmiş tamirlerine baktığında, Devices tablosundaki bir kayıt silinse veya adı değişse bile 
        (örneğin "Laptop" yerine "Dizüstü Bilgisayar" yapılsa bile), kullanıcının kendi girdiği orijinal adı görmesini sağlar.
         */
        public string DeviceName { get; set; }

        // Veritabanındaki cihazla eşleşirse Id'si buraya gelir
        public int? DeviceId { get; set; }
        public Device? Device { get; set; }

        // ML.NET veya kullanıcının seçtiği hedef zorluk seviyesi
        public string? TargetLevel { get; set; }

        // Durum takibi (Devam Ediyor, Tamamlandı, Başarısız)
        // ML.NET'i eğitmek için tamirin "başarısız" olup olmadığı kritik veridir.
        public string Status { get; set; } = "Pending";

        // LLM'den gelen ham JSON'u tutmaya yarayan prop. Her ihtimale karşı tutalım.
        public string? RawAiResponse { get; set; }

        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        //Bir isteğin birden çok adımı olabilir.
        public ICollection<RepairStep> Steps { get; set; }

        //İşlem anındaki zorluk puanını dondurur.
        public int DeviceDifficulty { get; set; }
        public bool IsTestData { get; set; } = true;

    }
}
