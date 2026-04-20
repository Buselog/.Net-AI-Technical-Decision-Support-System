namespace RepairGuidance.Domain.Exceptions
{
    public class RepairRequestNotFoundException : BaseBusinessException
    {
        public RepairRequestNotFoundException() : base("Tamir kaydı bulunamadı.")
        {

        }
    }
}
