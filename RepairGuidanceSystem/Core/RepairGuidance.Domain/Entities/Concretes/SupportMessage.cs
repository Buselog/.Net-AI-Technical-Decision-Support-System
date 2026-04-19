using RepairGuidance.Domain.Entities.Abstracts;

namespace RepairGuidance.Domain.Entities.Concretes
{
    public class SupportMessage : BaseEntity
    {
        public int RepairStepId { get; set; }
        public RepairStep RepairStep { get; set; } // Navigation Property
        public string Sender { get; set; } // "User" veya "AI"
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
    }
}
