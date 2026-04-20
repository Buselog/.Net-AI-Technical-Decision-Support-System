using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairGuidance.Application.Dtos;
using RepairGuidance.Application.Managers;

namespace RepairGuidance.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private readonly IAppUserManager _appUserManager;

        public AppUserController(IAppUserManager appUserManager)
        {
            _appUserManager = appUserManager;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var values = await _appUserManager.GetAllAsync();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var value = await _appUserManager.GetUserByIdAsync(id);
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(AppUserDto dto)
        {
            var value = await _appUserManager.AddAsync(dto);
            return Ok(value);
        }

        [HttpPut]
        public async Task<IActionResult> Update(AppUserDto dto)
        {
            await _appUserManager.UpdateAsync(dto);
            return Ok();
        }
    }
}
