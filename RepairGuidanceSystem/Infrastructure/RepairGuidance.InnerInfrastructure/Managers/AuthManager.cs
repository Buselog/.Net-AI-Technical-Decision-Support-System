using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepairGuidance.Application.Dtos;
using RepairGuidance.Application.Managers;
using RepairGuidance.Contract.Repositories;
using RepairGuidance.Domain.Entities.Concretes;
using RepairGuidance.Domain.Exceptions;
using RepairGuidance.InnerInfrastructure.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RepairGuidance.InnerInfrastructure.Managers
{
    public class AuthManager : IAuthManager
    {
        private readonly IAppUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthManager(IAppUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task RegisterAsync(UserRegisterDto dto)
        {
            // E-posta kontrolü
            var exists = await _userRepository.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (exists != null) throw new DuplicateEmailException();

            var user = new AppUser
            {
                FullName = $"{dto.FirstName} {dto.LastName}",
                Email = dto.Email,
                Password = PasswordHasher.HashPassword(dto.Password), // Şifre hashleme
                ExperienceScore = 0, // Yeni kullanıcı 0 puanla başlar
                ExperienceLevel = "Acemi",
                CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<LoginResponseDto> LoginAsync(UserLoginDto dto)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !PasswordHasher.VerifyPassword(dto.Password, user.Password))
            {
                throw new InvalidCredentialsException();
            }

            return new LoginResponseDto
            {
                Token = GenerateJwtToken(user),
                FullName = user.FullName,
                UserId = user.Id
            };
        }

        private string GenerateJwtToken(AppUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FullName", user.FullName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(5),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
