using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairGuidance.Application.Managers;

namespace RepairGuidance.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepairStepController : ControllerBase
    {
        private readonly IRepairStepManager _repairStepManager;

        public RepairStepController(IRepairStepManager repairStepManager)
        {
            _repairStepManager = repairStepManager;
        }

        //Kişi, ilgili tamirde bir adımı tamamladığında o step durumunu güncelleme:
        [HttpPatch("complete-step/{stepId}")]
        public async Task<IActionResult> CompleteStep(int stepId)
        {
            var step = await _repairStepManager.GetByIdAsync(stepId);
            if (step == null) return NotFound();

            step.IsCompleted = true;
            await _repairStepManager.UpdateAsync(step);
            return Ok("Adım başarıyla tamamlandı.");
        }



    }
}
