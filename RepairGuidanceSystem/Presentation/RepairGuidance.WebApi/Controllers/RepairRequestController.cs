using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairGuidance.Application.Dtos;
using RepairGuidance.Application.Managers;
using RepairGuidance.InnerInfrastructure.Managers;

namespace RepairGuidance.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepairRequestController : ControllerBase
    {
        private readonly IRepairRequestManager _repairRequestManager;

        public RepairRequestController(IRepairRequestManager repairRequestManager)
        {
            _repairRequestManager = repairRequestManager;
        }

       // Yeni bir tamir süreci başlatma:
        [HttpPost("start-repair")]
        public async Task<IActionResult> CreateRequest(CreateRepairRequestDto dto)
        {
            //Burada ileride AI entegrasyonu metodunu çağıracağız
            var result = await _repairRequestManager.CreateAiSupportGuidanceAsync(dto);
            return Ok(result);
        }

        [HttpGet("history/{userId}")]
        public IActionResult GetUserHistory(int userId)
        {
            var history = _repairRequestManager.Where(x => x.AppUserId == userId).ToList();
            return Ok(history);
        }

    }
}
