namespace RepairGuidance.Application.Dtos
{
    public class AiSupportRequestDto
    {
        public int RepairRequestId { get; set; }
        public int StepNumber { get; set; }
        public string UserQuestion { get; set; }
    }
}
