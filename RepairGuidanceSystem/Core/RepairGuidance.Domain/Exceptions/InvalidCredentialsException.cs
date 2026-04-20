namespace RepairGuidance.Domain.Exceptions
{
    public class InvalidCredentialsException : BaseBusinessException
    {
        public InvalidCredentialsException() : base("Geçersiz e-posta veya şifre.")
        {

        }
    }
}
