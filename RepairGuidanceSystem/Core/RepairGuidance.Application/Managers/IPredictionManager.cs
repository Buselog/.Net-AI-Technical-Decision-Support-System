using RepairGuidance.Application.Models;

namespace RepairGuidance.Application.Managers
{
    public interface IPredictionManager
    {

        // Veritabanındaki 5000 veriyi kullanarak modeli eğitir ve .zip olarak kaydeder
        Task TrainModelAsync();

        ModelOutput Predict(int difficulty, string targetLevel, float experienceScore);
    }
}

