namespace RepairGuidance.Domain.Exceptions
{
    public class AiServiceException : BaseBusinessException
    {
        public AiServiceException(string aiMessage) : base(aiMessage)
        {

        }
    }
}
