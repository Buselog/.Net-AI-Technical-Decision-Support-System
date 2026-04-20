namespace RepairGuidance.Domain.Exceptions
{
    public class DeviceNotEligibleException : BaseBusinessException
    {
        public DeviceNotEligibleException(string reason) : base($"Girilen cihaz kapsam dışı: {reason}")
        {

        }
    }
}
