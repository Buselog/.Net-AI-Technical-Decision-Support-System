using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairGuidance.Application.Dtos;
using RepairGuidance.Application.Managers;

namespace RepairGuidance.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserToolController : ControllerBase
    {
        private readonly IUserToolManager _userToolManager;

        public UserToolController(IUserToolManager userToolManager)
        {
            _userToolManager = userToolManager;
        }


        //Kişinin sahip olduğu aletlere yenisini ekler:
        [HttpPost]
        public async Task<IActionResult> AddToolToUser(UserToolDto dto)
        {
            var value = await _userToolManager.AddAsync(dto);
            return Ok(new {Message = value});
        }


        // Kişinin sahip olduğu aletleri getirir:
        [HttpGet("user/{userId}")] 
        public IActionResult GetUserInventory(int userId)
        {
            var tools = _userToolManager.Where(x => x.AppUserId == userId).ToList();
            return Ok(tools);
        }


    }
}
