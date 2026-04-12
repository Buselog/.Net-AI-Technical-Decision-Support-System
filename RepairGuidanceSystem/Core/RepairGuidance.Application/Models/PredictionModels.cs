using Microsoft.ML.Data;

namespace RepairGuidance.Application.Models
{
    public class ModelInput
    {
        public float DeviceDifficulty { get; set; }
        public float ExperienceScore { get; set; }
        public string TargetLevel { get; set; }

        [ColumnName("Label")]
        public bool Status { get; set; }
    }

    public class ModelOutput
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }
}