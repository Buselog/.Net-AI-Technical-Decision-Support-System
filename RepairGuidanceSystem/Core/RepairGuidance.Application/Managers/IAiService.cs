using RepairGuidance.Application.Models;
using RepairGuidance.Domain.Entities.Concretes;

namespace RepairGuidance.Application.Managers
{
    public interface IAiService
    {
        // Cihaz adı, sorun açıklaması ve kullanıcının alet listesini alıp 
        // bize bir metin (rehber) dönecek olan metodun imzası.

        Task<AiRepairResult> GetRepairGuidanceAsync(string userProblem, string deviceName, List<string> availableTools, string targetLevel, int deviceDifficulty);

        //Sistemde olmayan cihazlar için cihazı analiz eden ve uygunluk veren metot:
        Task<DeviceAnalysisResult> AnalyzeNewDeviceAsync(string deviceName);

        Task<string> GetStepSupportWithHistoryAsync(string deviceName, string problemDescription, string stepInstruction, List<SupportMessage> history, string newUserQuestion, int userExperienceScore, int deviceDifficulty, List<string> availableTools);
    }
}
