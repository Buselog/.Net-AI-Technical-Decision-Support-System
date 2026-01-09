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
        public string DeviceName { get; set; }

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

    }
}
