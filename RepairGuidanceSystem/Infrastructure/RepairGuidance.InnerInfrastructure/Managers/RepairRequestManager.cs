using AutoMapper;
using RepairGuidance.Application.Dtos;
using RepairGuidance.Application.Managers;
using RepairGuidance.Application.Models;
using RepairGuidance.Contract.Repositories;
using RepairGuidance.Domain.Entities.Concretes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairGuidance.InnerInfrastructure.Managers
{
    public class RepairRequestManager : BaseManager<RepairRequest, RepairRequestDto>, IRepairRequestManager
    {
        private readonly IAiService _aiService;
        private readonly IUserToolRepository _userToolRepository;

        public RepairRequestManager(IRepairRequestRepository repository, IMapper mapper, IAiService aiService, IUserToolRepository userToolRepository):base(repository, mapper)
        {
            _aiService = aiService;
            _userToolRepository = userToolRepository;
        }

       public async Task<RepairRequestDto> CreateAiSupportGuidanceAsync(CreateRepairRequestDto dto)
        {
            // Kullanıcının elindeki alet isimlerini db'den çekelim
            var tools = _userToolRepository.Where(x => x.AppUserId == dto.AppUserId).Select(x => x.Tool.Name).ToList();

            // 1. AI SERVİSİNİ ÇAĞIRMA (IAiService -> GroqAiService çalıştırır, Program.cs sayesinde)
            // GroqAiService'e gidiyoruz. O bize hem ham metni hem de parçalanmış Steps listesini içeren  "AiRepairResult" nesnesini dönüyor.

            var aiResult = await _aiService.GetRepairGuidanceAsync(dto.ProblemDescription, dto.DeviceName, tools, dto.TargetLevel);

            // Gelen veri ile beraber eksik verileri de doldurarak db kaydetmek
            // için entity'e çevirme ve db'ye kaydetme işlemi:
            var entity = _mapper.Map<RepairRequest>(dto);
            entity.RawAiResponse = aiResult.RawResponse;
            entity.Status = "InProgress";
            // PostgreSQL ve .NET arasındaki saat farkı sorununu (UTC) bu şekilde çözüyoruz.
            entity.CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            if (entity.Steps == null)
            {
                entity.Steps = new List<RepairStep>();
            }

            // 4. ADIMLARI (RepairStep) TEK TEK OLUŞTURMA
            for (int i = 0; i < aiResult.StepDetails.Count; i++)
            {
                var stepDetail = aiResult.StepDetails[i];

                var step = new RepairStep
                {
                    StepNumber = i + 1,
                    Instruction = stepDetail.Instruction, // Parçaladığımız talimat
                    ToolSuggestion = stepDetail.Tool,     // Parçaladığımız alet önerisi
                    IsCompleted = false
                };

                entity.Steps.Add(step);
            }

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            // Sonuç dto tipinde dönecek. Gereksiz veri kullanıcıya gözükmesin diye
            return _mapper.Map<RepairRequestDto>(entity);
        }

    }
}
