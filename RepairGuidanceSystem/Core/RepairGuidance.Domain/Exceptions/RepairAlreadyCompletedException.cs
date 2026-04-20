namespace RepairGuidance.Domain.Exceptions
{
    public class RepairAlreadyCompletedException : BaseBusinessException
    {
        public RepairAlreadyCompletedException() : base("Bu tamir süreci zaten başarıyla sonuçlanmış.")
        {

        }
    }
}
