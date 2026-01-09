using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.Application.Models;

public class AiRepairResult
{
    public string RawResponse { get; set; }
    public List<AiStepDetail> StepDetails { get; set; } = new();
}

public class AiStepDetail
{
    public string Instruction { get; set; }
    public string Tool { get; set; }
}
