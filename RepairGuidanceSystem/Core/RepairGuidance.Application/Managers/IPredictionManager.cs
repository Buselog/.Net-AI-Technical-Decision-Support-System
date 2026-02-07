using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Application.Managers
{
    public interface IPredictionManager
    {
        // Cihaz, kullanıcı tecrübesi ve hedef zorluğu alır, geriye başarı ihtimalini (0.0 - 1.0) döner.
        Task<float> GetSuccessProbabilityAsync(string deviceName, string experienceLevel, string targetLevel);
    }
}
