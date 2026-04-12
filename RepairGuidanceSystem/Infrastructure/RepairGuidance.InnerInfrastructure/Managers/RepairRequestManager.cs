using AutoMapper;
using RepairGuidance.Application.Dtos;
using RepairGuidance.Application.Managers;
using RepairGuidance.Contract.Repositories;
using RepairGuidance.Domain.Entities.Concretes;

namespace RepairGuidance.InnerInfrastructure.Managers
{
    public class RepairRequestManager : BaseManager<RepairRequest, RepairRequestDto>, IRepairRequestManager
    {
        private readonly IAiService _aiService;
        private readonly IUserToolRepository _userToolRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IPredictionManager _predictionManager;

        public RepairRequestManager(IRepairRequestRepository repository, IMapper mapper, IAiService aiService, IUserToolRepository userToolRepository, IDeviceRepository deviceRepository, IPredictionManager predictionManager):base(repository, mapper)
        {
            _aiService = aiService;
            _userToolRepository = userToolRepository;
            _deviceRepository = deviceRepository;
            _predictionManager = predictionManager;
        }

        public async Task<RepairRequestDto> CreateAiSupportGuidanceAsync(CreateRepairRequestDto dto)
        {
            // --- 1. ADIM: CİHAZ KONTROLÜ VE ÖĞRENME (GATEKEEPER) ---
            // Veritabanında bu isimde veya benzer isimde bir cihaz var mı?
            var device = await _deviceRepository.FindBestMatchAsync(dto.DeviceName);

            int appliedDifficulty;
            int? finalDeviceId = null;

            if (device != null)
            {
                // Senaryo A: Cihaz sistemde zaten var.
                appliedDifficulty = device.DifficultyScore;
                finalDeviceId = device.Id;
            }
            else
            {
                // Senaryo B: Cihaz sistemde yok. AI'ya analiz ettirip öğreneceğiz.
                var analysis = await _aiService.AnalyzeNewDeviceAsync(dto.DeviceName);

                if (!analysis.IsEligible)
                {
                    // Eğer AI "Uzay Roketi uygun değil" dediyse burada işlemi kesiyoruz.
                    throw new Exception($"Tamir Kapsamı Dışı: {analysis.AnalysisReason}");
                }

                // AI uygun dedi; cihazı veritabanına ekle (Öğrenme Aşaması)
                var newDevice = await _deviceRepository.CreateAndReturnDeviceAsync(
                    dto.DeviceName,
                    analysis.DifficultyScore,
                    analysis.ToolCategoryId);

                appliedDifficulty = newDevice.DifficultyScore;
                finalDeviceId = newDevice.Id;
            }

            // --- 2. ADIM: ML.NET BAŞARI TAHMİNİ ---
            // Mühürlü modelimizi kullanarak kullanıcının bu cihazı tamir etme olasılığını hesaplıyoruz.
            var prediction = _predictionManager.Predict(appliedDifficulty, dto.TargetLevel);

            // --- 3. ADIM: AI REHBER ÜRETİMİ (MEVCUT MANTIĞIN) ---
            var tools = _userToolRepository.Where(x => x.AppUserId == dto.AppUserId)
                                           .Select(x => x.Tool.Name).ToList();

            // AI'ya cihaz adını ve zorluğunu da net şekilde bildiriyoruz.
            var aiResult = await _aiService.GetRepairGuidanceAsync(
                dto.ProblemDescription,
                dto.DeviceName,
                tools,
                dto.TargetLevel);

            // --- 4. ADIM: KAYIT VE ENTITY MAPPING ---
            var entity = _mapper.Map<RepairRequest>(dto);

            // Yeni eklediğimiz property'leri set edelim (Daha önce konuştuğumuz snapshot mantığı)
            entity.DeviceId = finalDeviceId;
            entity.DeviceDifficulty = appliedDifficulty;
            // entity.SuccessProbability = prediction.Score; // DB'de kolonun varsa ekle

            entity.RawAiResponse = aiResult.RawResponse;
            entity.Status = "InProgress";
            entity.CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            if (entity.Steps == null) entity.Steps = new List<RepairStep>();

            // Adımları döngü ile ekleme (Mevcut kodun)
            for (int i = 0; i < aiResult.StepDetails.Count; i++)
            {
                var stepDetail = aiResult.StepDetails[i];
                entity.Steps.Add(new RepairStep
                {
                    StepNumber = i + 1,
                    Instruction = stepDetail.Instruction,
                    ToolSuggestion = stepDetail.Tool,
                    IsCompleted = false
                });
            }

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<RepairRequestDto>(entity);
        }

        //public async Task<RepairRequestDto> CreateAiSupportGuidanceAsync(CreateRepairRequestDto dto)
        // {
        //     // Kullanıcının elindeki alet isimlerini db'den çekelim
        //     var tools = _userToolRepository.Where(x => x.AppUserId == dto.AppUserId).Select(x => x.Tool.Name).ToList();

        //     // 1. AI SERVİSİNİ ÇAĞIRMA (IAiService -> GroqAiService çalıştırır, Program.cs sayesinde)
        //     // GroqAiService'e gidiyoruz. O bize hem ham metni hem de parçalanmış Steps listesini içeren  "AiRepairResult" nesnesini dönüyor.

        //     var aiResult = await _aiService.GetRepairGuidanceAsync(dto.ProblemDescription, dto.DeviceName, tools, dto.TargetLevel);

        //     // Gelen veri ile beraber eksik verileri de doldurarak db kaydetmek
        //     // için entity'e çevirme ve db'ye kaydetme işlemi:
        //     var entity = _mapper.Map<RepairRequest>(dto);
        //     entity.RawAiResponse = aiResult.RawResponse;
        //     entity.Status = "InProgress";
        //     // PostgreSQL ve .NET arasındaki saat farkı sorununu (UTC) bu şekilde çözüyoruz.
        //     entity.CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

        //     if (entity.Steps == null)
        //     {
        //         entity.Steps = new List<RepairStep>();
        //     }

        //     // 4. ADIMLARI (RepairStep) TEK TEK OLUŞTURMA
        //     for (int i = 0; i < aiResult.StepDetails.Count; i++)
        //     {
        //         var stepDetail = aiResult.StepDetails[i];

        //         var step = new RepairStep
        //         {
        //             StepNumber = i + 1,
        //             Instruction = stepDetail.Instruction, // Parçaladığımız talimat
        //             ToolSuggestion = stepDetail.Tool,     // Parçaladığımız alet önerisi
        //             IsCompleted = false
        //         };

        //         entity.Steps.Add(step);
        //     }

        //     await _repository.AddAsync(entity);
        //     await _repository.SaveChangesAsync();

        //     // Sonuç dto tipinde dönecek. Gereksiz veri kullanıcıya gözükmesin diye
        //     return _mapper.Map<RepairRequestDto>(entity);
        // }

    }
}
