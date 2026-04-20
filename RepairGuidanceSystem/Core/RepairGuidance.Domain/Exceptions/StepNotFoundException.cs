namespace RepairGuidance.Domain.Exceptions
{
    public class StepNotFoundException : BaseBusinessException
    {
        public StepNotFoundException() : base("İlgili adım kaydı bulunamadı.")
        {

        }
    }
}
