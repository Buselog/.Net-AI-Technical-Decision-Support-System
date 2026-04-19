using Microsoft.Extensions.Configuration;
using RepairGuidance.Application.Managers;
using RepairGuidance.Application.Models;
using RepairGuidance.Domain.Entities.Concretes;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace RepairGuidance.Infrastructure.ExternalServices
{


    public class GroqAiService : IAiService
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;

        public GroqAiService(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            // API Key, appsettings.json dosyasından IConfiguration aracılığıyla güvenli bir şekilde okunur.
            _apiKey = configuration["GroqApi:ApiKey"];
        }

        public async Task<AiRepairResult> GetRepairGuidanceAsync(string userProblem, string deviceName, List<string> availableTools, string targetLevel, int deviceDifficulty)
        {
            var url = "https://api.groq.com/openai/v1/chat/completions";

            // 1. YETKİLENDİRME (Authorization)
            // Bearer token standardı kullanılarak API anahtarı her isteğin header kısmına eklenir.
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            // 2. İSTEK GÖVDESİ (Request Body)
            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                    {
            new {
                role = "system",
                content = $@"Sen profesyonel bir tamir asistanısın. Öncelikle sadece Türkçe cevap ver. 
                          Kullanıcı şu an [Cihaz Zorluğu: {deviceDifficulty}] olan bir cihaz için 
                          [{targetLevel}] seviyesinde bir rehber talep etti.

                            Cevaplarını MUTLAKA VE KESİNLİKLE şu formatta ver: 
                            [Adım X: Talimat Metni | Alet: Alet Adı]
                            Adımları KESİNLİKLE {targetLevel} seviyesindeki birinin anlayacağı zorlukta hazırla. 
                            Ancak unutma; cihazın zorluğu {deviceDifficulty} seviyesinde. Eğer seviye farkı yüksekse, adımlara 'DİKKAT:' ile başlayan güvenlik notları ekle.
                            Talimatları KESİNLİKLE kullanıcının elindeki aletleri ({string.Join(", ", availableTools)}) kullanarak çözebileceği şekilde kurgula.
                            Eğer arızayı gidermek için kullanıcının elinde olmayan bir alet ŞART ise (alternatifi yoksa), adımın alet kısmına 'GEREKLİ: [Alet Adı]' yaz ve talimatta neden gerektiğini açıkla.
                            Başka hiçbir açıklama metni ekleme, sadece bu formatta adımları sırala."

            },
                    new {
                        role = "user",
                content = $"Cihaz: {deviceName}. Sorun: {userProblem}. Kullanıcı Seviyesi: {targetLevel}.  Elindeki Aletler: {string.Join(", ", availableTools)}."
            }
        },
                temperature = 0.2
            };

            var response = await _client.PostAsJsonAsync(url, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"AI Servis Hatası: {response.StatusCode} - {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<GroqResponse>();
            var fullText = result?.choices?[0]?.message?.content ?? "";

            return ParseAiResponse(fullText);
        }

        public async Task<DeviceAnalysisResult> AnalyzeNewDeviceAsync(string deviceName)
        {
            var url = "https://api.groq.com/openai/v1/chat/completions";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                 new {
                role = "system",
                content = @"Sen, 'RepairGuidance' adlı profesyonel bir tamir karar destek sisteminin uzman analiz modülüsün. 
                           Görevin, kullanıcının girdiği yeni bir cihazı sistemin kapsamına ve matematiksel zorluk skalasına göre değerlendirmektir.

                           ### 1. PROJE KAPSAMI VE UYGUNLUK (isEligible)
                           Bir cihazın tamir edilebilir (true) sayılması için şu kriterleri karşılaması gerekir:
                           - Ev ortamında, standart el aletleri veya temel elektronik ekipmanlarla müdahale edilebilir olmalı.
                           - Hayati risk taşıyan çok yüksek gerilimli sistemler (şehir şebeke trafosu vb.) veya aşırı karmaşık endüstriyel makineler (nükleer reaktör, uzay teknolojileri, ağır sanayi presleri) OLMAMALIDIR.

                           ### 2. KATEGORİ LİSTESİ (categoryId)
                           Cihazı MUTLAKA aşağıdaki 15 kategoriden en uygun olanına ata:
                           1: Genel El Aletleri, 2: Elektrikli Güç Aletleri, 3: Ölçüm ve Test Cihazları, 4: Elektrik Tesisat Malzemeleri, 5: Sıhhi Tesisat ve Boru, 
                           6: Beyaz Eşya Mekanik Parçalar, 7: Küçük Ev Aletleri Onarım, 8: Elektronik ve Lehimleme, 9: Bilgisayar ve Donanım, 
                           10: İklimlendirme (HVAC), 11: Bahçe ve Dış Mekan Bakım, 12: Güvenlik ve Kilit Sistemleri, 13: Mobilya Montaj ve Bağlantı, 
                           14: Temizlik ve Bakım Kimyasalları, 15: İş Güvenliği Ekipmanları.

                           ### 3. MATEMATİKSEL ZORLUK SKALASI (difficulty: 1-100)
                           Verdiğin puan, sistemdeki ML.NET modelini besleyecektir. Şu referansları baz alarak lineer bir puanlama yap:
                           - ÇOK KOLAY (10-30): Gardırop (30), Ofis Koltuğu (30), Su Isıtıcı(30), Ütü (30).
                           - KOLAY/ORTA (30-50): Blender(35), Saç Kurutma Makinesi (35), Mikser (35), Tost Makinesi(40), Vantilatör(40), Tansiyon Aleti(40) ,Priz (40), Aydınlatma Armatürü(45), Dikiş Makinesi(45), Elektrikli Süpürge(40), Kahve Makinesi(40).
                           - ORTA/ZOR (50-70): Musluk (50), Bulaşık Makinesi (60), Fırın(60), Yazıcı (60), Buzdolabı(65), Ankastre Ocak(55), Bahçe Pompası(55), Barkod Okuyucu(60), Basınçlı Yıkama Makinesi(60), Derin Dondurucu(65) Duş Bataryası(55), Elektrikli Scooter(60), UPS(65).
                           - ZOR (70-85): Laptop (80), Drone (80), Alarm Sistemi(75), Kombi (80), 3D Yazıcı (75), Masaüstü Bilgisayar(75), Oyun Konsolu(75) Sigorta Panosu(75), Tablet(75).
                           - ÇOK ZOR (85-100): Hassas anakart onarımları veya karmaşık motor blokları.

                           ### ÇIKTI FORMATI
                           Sadece aşağıdaki JSON formatında yanıt ver, başka açıklama ekleme:
                           {
                             ""isEligible"": bool,
                             ""categoryId"": int,
                             ""difficulty"": int,
                             ""reason"": ""Neden bu kategori ve puanı seçtiğinin teknik açıklaması.""
                            }"
                  },
                   new { role = "user", content = $"Cihaz: {deviceName}" }
                 },
                response_format = new { type = "json_object" }, // JSON dönmesini zorunlu kılıyoruz
                temperature = 0.1 // Daha tutarlı sonuçlar için düşük ısı
            };

            var response = await _client.PostAsJsonAsync(url, requestBody);
            var result = await response.Content.ReadFromJsonAsync<GroqResponse>();
            var jsonText = result?.choices?[0]?.message?.content;

            // JSON metnini DTO'ya map ediyoruz
            var analysis = JsonSerializer.Deserialize<DeviceAnalysisResult>(jsonText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return analysis;
        }

        private AiRepairResult ParseAiResponse(string rawText)
        {
            var result = new AiRepairResult { RawResponse = rawText };

            // YENİ REGEX: [Adım X: ... | Alet: ...] formatını yakalar.
            // (.*?) -> Talimatı, (.*?) -> Aleti temsil eder.
            // Bu Regex hem köşeli parantezli hem parantezsiz halini yakalar.
            var matches = Regex.Matches(rawText, @"Adım\s*\d+:(.*?) \|\s*Alet:(.*?)(?=\n|Adım|$)", RegexOptions.Singleline);


            foreach (Match match in matches)
            {
                var instruction = match.Groups[1].Value.Trim();
                var tool = match.Groups[2].Value.Trim();

                if (!string.IsNullOrEmpty(instruction))
                {
                    // AiRepairResult sınıfına Steps'in yanına bir de ToolSuggestions listesi ekleyeceğiz 
                    // ya da daha iyisi bunları bir nesne olarak tutacağız.
                    result.StepDetails.Add(new AiStepDetail
                    {
                        Instruction = instruction,
                        Tool = tool
                    });
                }
            }
            return result;
        }

        public async Task<string> GetStepSupportWithHistoryAsync(string deviceName, string problemDescription, string stepInstruction, List<SupportMessage> history, string newUserQuestion, int userExperienceScore, int deviceDifficulty, List<string> availableTools)
        {
            var url = "https://api.groq.com/openai/v1/chat/completions";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            // 1. Geçmiş mesajları AI'nın anlayacağı formata çeviriyoruz
            var messages = new List<object>
             {
              new {
            role = "system",
            content = $@"Sen profesyonel bir tamir uzmanı ve dünyanın en iyi teknik destek asistanısın. 
                         Şu an kullanıcıya, kullanıcının tecrübe seviyesine ve cihazın zorluğuna göre spesifik bir tamir adımında yardımcı oluyor ve çözümler sunuyorsun..
                         
                         BAĞLAM VE SEVİYE BİLGİLERİ:
                         - Cihaz: {deviceName} (Zorluk Skoru: {deviceDifficulty}/100)
                         - Kullanıcı Tecrübe Puanı: {userExperienceScore}/100
                         - Kullanıcının Elindeki Aletler: {string.Join(", ", availableTools)}.
                         - Genel Arıza: {problemDescription}
                         - Mevcut Talimat: {stepInstruction}

                         KRİTİK TALİMATLAR:
                         1. ÇÖZÜM ODAKLI ENVANTER: Tavsiye verirken ÖNCELİKLE kullanıcının elindeki aletleri kullanmasını sağla.
                         2. EKSİK ALETLER: Eğer elinde olmayan bir alet öneriyorsan, bunun bir 'Yatırım/Satın Alma Tavsiyesi' olduğunu belirt.
                         3. SEVİYEYE UYGUNLUK: Kullanıcının tecrübe puanı düşükse (0-40) evdeki basit yöntemleri ve güvenliği ön plana çıkar. Puan yüksekse (70+) daha teknik, profesyonel aletler gerektiren ve biraz daha ileri düzey olabilecek çözümler öner.
                         4. HAFIZA KONTROLÜ: 'history' içindeki mesajları oku. Eğer kullanıcı bir önerini denediğini ve olmadığını söylüyorsa, o öneriyi ve benzerlerini (örneğin ısıtma dedinse ve olmadıysa, fön makinesi/pürmüz gibi her türlü ısıtma türevini) LİSTENDEN SİL.
                         5. DİL VE TON: 
                            {userExperienceScore} puanına uygun jargon seç. Yani:
                          - Kullanıcı Puanı <= 40 ise: Terimleri açıkla (Örn: 'Multimetreyi bip sesinin geldiği kısa devre moduna al'), çok sabırlı ol.
                          - Kullanıcı Puanı >= 70 ise: Eğer kullanıcı puanı 70'den büyük ise teknik jargonu kullan. 70-80 puan aralığında teknik jargon konusunda yine biraz tedbirli olabilirsin; ama 80 ve üzeri puanda teknik jargonu kullanabilirsin artık. (Örn: 'Süreklilik modunda empedans kontrolü yap'), doğrudan sonuca git.
                         6. REDDETME VE EMPATİ: Kullanıcının 'olmadı' dediği her durumda 'Anlıyorum, [Yöntem] sonuç vermediyse...' diyerek söze başla.
                         7. KISA VE NET OL: Teknik terimleri kullanıcının seviyesine göre açıkla veya doğrudan kullan.
                         8. ASLA İngilizce kelime (slightly, necessary vb.) kullanma. Saf Türkçe konuş."
                         

        }
              };

            foreach (var msg in history)
            {
                messages.Add(new { role = msg.Sender == "User" ? "user" : "assistant", content = msg.Content });
            }

            // 2. Yeni soruyu ekliyoruz
            messages.Add(new { role = "user", content = newUserQuestion });

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = messages.ToArray(),
                temperature = 0.4
            };

            var response = await _client.PostAsJsonAsync(url, requestBody);
            var result = await response.Content.ReadFromJsonAsync<GroqResponse>();
            return result?.choices?[0]?.message?.content ?? "Destek alınamadı.";
        }

    }

    // --- DATA TRANSFER OBJECTS (DTO) ---
    // Bu sınıflar sadece API yanıtını C# nesnesine dönüştürmek (Mapping) için kullanılır.

    public class GroqResponse
    {
        public List<Choice> choices { get; set; }
    }

    public class Choice
    {
        public Message message { get; set; }
    }

    public class Message
    {
        public string content { get; set; }
    }


}