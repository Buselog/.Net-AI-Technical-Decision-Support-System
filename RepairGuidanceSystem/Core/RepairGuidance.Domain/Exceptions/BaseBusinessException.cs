namespace RepairGuidance.Domain.Exceptions
{
    public class BaseBusinessException : Exception
    {
        public BaseBusinessException(string message) : base(message)
        {

        }
    }
}
