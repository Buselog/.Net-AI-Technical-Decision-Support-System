using RepairGuidance.Domain.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Domain.Entities.Concretes
{
    public class Device : BaseEntity
    {
        //Cihazın tam adı
        public string Name { get; set; }

        // ML.NET modeline gönderilecek zorluk puanı (Örn: 80, 60, 30)
        public int DifficultyScore { get; set; }

        // ToolCategory tablosuyla ilişki
        // Bu sayede bilinmeyen cihazları kategori üzerinden analiz edilir.
        public int ToolCategoryId { get; set; }
        public ToolCategory ToolCategory { get; set; }

        // Sistem tarafından mı tanımlandı yoksa kullanıcı mı ekledi ?
        public bool IsPredefined { get; set; } = true;

        // Bu cihaza ait yapılan tüm talepler (İlişki için)
        public ICollection<RepairRequest> RepairRequests { get; set; }



    }
}
