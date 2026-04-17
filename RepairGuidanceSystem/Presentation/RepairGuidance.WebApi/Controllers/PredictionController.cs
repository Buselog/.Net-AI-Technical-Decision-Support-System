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

        // 1. Eğitimi Tetikle: Yeni sayısal modelini oluşturmak için bunu bir kez çalıştır.
        [HttpGet("train")]
        public async Task<IActionResult> Train()
        {
            await _predictionManager.TrainModelAsync();
            return Ok("Model başarıyla eğitildi ve sayısal skorlar (Difficulty) baz alınarak kaydedildi.");
        }

        // 2. Tahmin Testi: Yeni 'Predict' metodunu deniyoruz.
        [HttpGet("test-predict")]
        public IActionResult Test(int difficulty = 80, string targetLevel = "Acemi", float experienceScore = 30)
        {
            var result = _predictionManager.Predict(difficulty, targetLevel, experienceScore);

            return Ok(new
            {
                Input = new { DeviceDifficulty = difficulty, UserLevel = targetLevel, ExperienceScore = experienceScore },
                Prediction = result.Prediction ? "Başarılı Olabilir" : "Başarısız Olabilir",
                SuccessProbability = $"%{result.Probability * 100:F2}",
                RawScore = result.Score // Modelin ürettiği ham skor
            });
        }
    }
}
