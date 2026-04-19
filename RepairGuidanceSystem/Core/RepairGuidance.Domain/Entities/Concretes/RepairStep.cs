using RepairGuidance.Domain.Entities.Abstracts;

namespace RepairGuidance.Domain.Entities.Concretes
{
    public class RepairStep : BaseEntity
    {
        public int RepairRequestId { get; set; }
        public RepairRequest RepairRequest { get; set; }
        public int StepNumber { get; set; }

        // Bulunulan adımın içerdiği talimat bilgisi
        public string Instruction { get; set; } // "Vidayı sökün"

        // İlgili adımda lazım olan alet
        /*
        Eğer kullanıcı 'Acemi' ise sistem basit aletler (tornavida gibi) önerilen adımlar üretiyor, 
        'Uzman'ise daha teknik aletler (multimetre, havya) öneriyor.

        ToolSuggestion, sistemin kişiye özel olduğunun bir metriğidir.
         */
        public string? ToolSuggestion { get; set; }
        public bool IsCompleted { get; set; }

        public ICollection<SupportMessage> SupportMessages { get; set; }

    }
}
