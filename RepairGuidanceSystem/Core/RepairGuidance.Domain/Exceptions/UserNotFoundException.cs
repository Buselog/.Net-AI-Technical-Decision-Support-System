namespace RepairGuidance.Domain.Exceptions
{
    public class UserNotFoundException : BaseBusinessException
    {
        public UserNotFoundException() : base("Kullanıcı bulunamadı.")
        {

        }
    }
}
