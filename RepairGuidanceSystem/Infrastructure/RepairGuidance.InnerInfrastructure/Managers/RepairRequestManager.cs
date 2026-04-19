using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        private readonly IAppUserRepository _appUserRepository;
        private readonly IRepairStepRepository _repairStepRepository;
        private readonly ISupportMessageRepository _supportMessageRepository;

        public RepairRequestManager(IRepairRequestRepository repository, IMapper mapper, IAiService aiService, IUserToolRepository userToolRepository, IDeviceRepository deviceRepository, IPredictionManager predictionManager, IAppUserRepository appUserRepository, IRepairStepRepository repairStepRepository, ISupportMessageRepository supportMessageRepository) : base(repository, mapper)
        {
            _aiService = aiService;
            _userToolRepository = userToolRepository;
            _deviceRepository = deviceRepository;
            _predictionManager = predictionManager;
            _appUserRepository = appUserRepository;
            _repairStepRepository = repairStepRepository;
            _supportMessageRepository = supportMessageRepository;
        }

        public async Task<RepairRequestDto> CreateAiSupportGuidanceAsync(CreateRepairRequestDto dto)
        {
            var device = await _deviceRepository.FindBestMatchAsync(dto.DeviceName);
            int appliedDifficulty;
            int? finalDeviceId = null;

            if (device != null)
            {
                appliedDifficulty = device.DifficultyScore;
                finalDeviceId = device.Id;
            }
            else
            {
                var analysis = await _aiService.AnalyzeNewDeviceAsync(dto.DeviceName);
                if (!analysis.IsEligible) throw new Exception($"Kapsam Dışı: {analysis.AnalysisReason}");

                // Yeni cihazı ekliyoruz
                var newDevice = await _deviceRepository.CreateAndReturnDeviceAsync(
                    dto.DeviceName, analysis.DifficultyScore, analysis.ToolCategoryId);
                appliedDifficulty = newDevice.DifficultyScore;
                finalDeviceId = newDevice.Id;
            }

            // Kullanıcının gerçek tecrübe puanını alalım (Tahmin kalitesi için)
            var user = await _appUserRepository.GetByIdAsync(dto.AppUserId); // Veya uygun repo üzerinden çekin
            int userScore = user?.ExperienceScore ?? 50;


            // ML Tahmini (ModelInput artık zorluk ve gerçek kullanıcı puanını alıyor)
            var prediction = _predictionManager.Predict(appliedDifficulty, dto.TargetLevel, userScore);

            var tools = _userToolRepository.Where(x => x.AppUserId == dto.AppUserId).Select(x => x.Tool.Name).ToList();
            var aiResult = await _aiService.GetRepairGuidanceAsync(dto.ProblemDescription, dto.DeviceName, tools, dto.TargetLevel, appliedDifficulty);

            var entity = _mapper.Map<RepairRequest>(dto);
            entity.Id = 0;
            entity.SuccessProbability = (decimal)prediction.Probability;
            entity.DeviceId = finalDeviceId;
            entity.DeviceDifficulty = appliedDifficulty;

            // entity.SuccessProbability = prediction.Probability; // Entity'de bu alan varsa açın

            entity.RawAiResponse = aiResult.RawResponse;
            entity.Status = "InProgress";
            entity.CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            // Adımları ekleme
            entity.Steps = aiResult.StepDetails.Select((sd, index) => new RepairStep
            {
                StepNumber = index + 1,
                Instruction = sd.Instruction,
                ToolSuggestion = sd.Tool,
                IsCompleted = false
            }).ToList();

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<RepairRequestDto>(entity);
        }


        public async Task<string> CompleteRepairRequestAsync(int requestId)
        {
            // 1. Talebi getir
            var request = await _repository.GetByIdAsync(requestId);
            if (request == null) return "Tamir kaydı bulunamadı.";
            if (request.Status == "Completed") return "Bu tamir zaten başarıyla sonuçlanmış.";

            // 2. Status Güncelle
            request.Status = "Completed";
            _repository.Update(request);

            // 3. Kullanıcıya Tecrübe Puanı Ekle
            var user = await _appUserRepository.GetByIdAsync(request.AppUserId);
            if (user != null)
            {
                // Algoritma: Cihaz Zorluğu / 20 kadar puan ekle (Örn: Drone 80 ise +8 puan)
                int earnedPoints = CalculateEarnedPoints(request.DeviceDifficulty, user.ExperienceScore);
                user.ExperienceScore += earnedPoints;

                // Puan arttıkça Seviye (Level) isimlendirmesini güncelle
                if (user.ExperienceScore >= 75) user.ExperienceLevel = "Uzman";
                else if (user.ExperienceScore >= 40) user.ExperienceLevel = "Orta";

                _appUserRepository.Update(user);
            }

            await _repository.SaveChangesAsync();
            return $"Tebrikler! Tamiri başarıyla bitirdiniz ve {request.DeviceDifficulty / 10} tecrübe puanı kazandınız.";
        }


        private int CalculateEarnedPoints(int deviceDifficulty, int userCurrentScore)
        {
            // Temel puan (80 zorluk için 4 puan)
            double basePoints = deviceDifficulty / 20.0;

            // Meydan okuma bonusu: Eğer cihaz zorluğu kullanıcının puanından yüksekse
            if (deviceDifficulty > userCurrentScore)
            {
                basePoints += 2;
            }

            // En az 1 puan, en fazla 10 puan (limit koymak güvenlidir)
            return (int)Math.Clamp(basePoints, 1, 10);
        }

        public async Task<string> GetSupportForStepAsync(AiSupportRequestDto dto)
        {
            // 1. Adımı Getir (Include işlemi Repository katmanında veya burada IQueryable ile yapılmalı)
            // Onion Architecture'da navigasyon propertyleri (Include) için Repository metodunu kullanıyoruz.
            var step = await _repairStepRepository.Where(s => s.RepairRequestId == dto.RepairRequestId && s.StepNumber == dto.StepNumber)
                                                   .Include(s => s.RepairRequest)
                                                   .Include(s => s.SupportMessages)
                                                   .FirstOrDefaultAsync();

            if (step == null) return "Adım bilgisi bulunamadı.";

            var user = await _appUserRepository.GetByIdAsync(step.RepairRequest.AppUserId);
            int userScore = user?.ExperienceScore ?? 50;

            var tools = _userToolRepository
               .Where(x => x.AppUserId == step.RepairRequest.AppUserId)
               .Select(x => x.Tool.Name)
               .ToList();

            // 2. AI'dan destek al (Geçmiş mesajları tarih sırasına göre gönderiyoruz)
            var aiAnswer = await _aiService.GetStepSupportWithHistoryAsync(
                step.RepairRequest.DeviceName,
                step.RepairRequest.ProblemDescription,
                step.Instruction,
                step.SupportMessages.OrderBy(m => m.SentAt).ToList(),
                dto.UserQuestion,
                userScore,                         
                step.RepairRequest.DeviceDifficulty,
                tools);

            // 3. Mesajları Repository üzerinden kaydet
            var userMsg = new SupportMessage { RepairStepId = step.Id, Sender = "User", Content = dto.UserQuestion };
            var aiMsg = new SupportMessage { RepairStepId = step.Id, Sender = "AI", Content = aiAnswer };

            await _supportMessageRepository.AddAsync(userMsg);
            await _supportMessageRepository.AddAsync(aiMsg);

            // Değişiklikleri fiziksel olarak kaydet
            await _supportMessageRepository.SaveChangesAsync();

            return aiAnswer;
        }

    }

 }



