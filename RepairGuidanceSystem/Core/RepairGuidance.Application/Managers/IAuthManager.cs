
using RepairGuidance.Application.Dtos;

namespace RepairGuidance.Application.Managers
{
    public interface IAuthManager
    {
        Task RegisterAsync(UserRegisterDto registerDto);
        Task<LoginResponseDto> LoginAsync(UserLoginDto loginDto);
    }
}
