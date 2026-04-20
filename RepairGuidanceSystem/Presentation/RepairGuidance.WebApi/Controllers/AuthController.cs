using Microsoft.AspNetCore.Mvc;
using RepairGuidance.Application.Dtos;
using RepairGuidance.Application.Managers;

namespace RepairGuidance.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto)
        {
            await _authManager.RegisterAsync(registerDto);
            return Ok(new { Message = "Kullanıcı kaydı başarıyla tamamlandı." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            var response = await _authManager.LoginAsync(loginDto);
            return Ok(response);
        }
    }
}
