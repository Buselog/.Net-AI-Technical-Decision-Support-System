using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Application.Models
{

    /*
     AI servisinden dönecek olan "Bu cihaz uygun mu? Zorluğu ne? Kategorisi ne?" gibi karmaşık yanıtı, 
     kodun içinde düzenli bir nesne olarak taşımak için.
     */
    public class DeviceAnalysisResult
    {
        // AI'nın cihazı kapsama uygun bulup bulmadığı (true/false)
        public bool IsEligible { get; set; }

        // AI'nın belirlediği 1-100 arası zorluk puanı
        public int DifficultyScore { get; set; }

        // AI'nın seçtiği kategori ID'si (Senin 1-15 arası tablondaki ID)
        public int ToolCategoryId { get; set; }

        // AI'nın neden uygun bulduğuna veya bulmadığına dair kısa not
        public string AnalysisReason { get; set; }
    }
}
