
namespace RepairGuidance.Application.Dtos
{
    public class UserLoginDto : IDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
