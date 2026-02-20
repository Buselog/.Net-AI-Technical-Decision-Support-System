using RepairGuidance.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Application.Managers
{
    public interface IAiService
    {
        // Cihaz adı, sorun açıklaması ve kullanıcının alet listesini alıp 
        // bize bir metin (rehber) dönecek olan metodun imzası.

        Task<AiRepairResult> GetRepairGuidanceAsync(string userProblem, string deviceName, List<string> availableTools, string targetLevel);

        //Sistemde olmayan cihazlar için cihazı analiz eden ve uygunluk veren metot:
        Task<DeviceAnalysisResult> AnalyzeNewDeviceAsync(string deviceName);
    }
}
