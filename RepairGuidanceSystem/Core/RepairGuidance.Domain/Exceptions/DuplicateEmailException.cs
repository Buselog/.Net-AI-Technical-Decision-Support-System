namespace RepairGuidance.Domain.Exceptions
{
    public class DuplicateEmailException : BaseBusinessException
    {
        public DuplicateEmailException () : base("Bu e-posta adresi zaten alınmış.")
        {

        }
    }
}
