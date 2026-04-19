
namespace RepairGuidance.Application.Dtos
{
    public class LoginResponseDto : IDto
    {
        public string Token { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
