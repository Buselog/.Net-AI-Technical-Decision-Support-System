using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepairGuidance.Application.Managers;

namespace RepairGuidance.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly IPredictionManager _predictionManager;

        public PredictionController(IPredictionManager predictionManager)
        {
            _predictionManager = predictionManager;
        }

        // 1. Eğitimi Tetikle: Tarayıcıdan bu adrese gittiğinde 5000 satır işlenecek.
        [HttpGet("train")]
        public async Task<IActionResult> Train()
        {
            await _predictionManager.TrainModelAsync();
            return Ok("Model başarıyla eğitildi ve 'repair_model.zip' olarak kaydedildi.");
        }

        // 2. Tahmin Yap: Örnek bir cihaz ve seviye gönderip başarı oranını soralım.
        [HttpGet("test-predict")]
        public async Task<IActionResult> Test()
        {
            // Örnek Senaryo: Acemi bir kullanıcı, Uzmanlık gerektiren bir Laptop tamirinde ne kadar başarılı olur?
            var probability = await _predictionManager.GetSuccessProbabilityAsync("Gardırop", 95, "Uzman");

            return Ok(new
            {
                Message = "Tahmin Sonucu",
                SuccessProbability = $"%{probability * 100:F2}" // Örn: %12.45 şeklinde formatlar.
            });
        }
    }
}
