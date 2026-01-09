using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairGuidance.Application.Dtos;
using RepairGuidance.Application.Managers;

namespace RepairGuidance.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToolController : ControllerBase
    {
        private readonly IToolManager _toolManager;

        public ToolController(IToolManager toolManager)
        {
            _toolManager = toolManager;
        }

        // Sistemdeki tüm aletleri listeler (Seçim listesi için)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var values = await _toolManager.GetAllAsync();
            return Ok(values);
        }

        // Sisteme yeni bir alet tipi ekler (Admin işlevi)
        [HttpPost]
        public async Task<IActionResult> Create(ToolDto dto)
        {
            var value = await _toolManager.AddAsync(dto);
            return Ok(new { Message = value });
        }

    }
}
