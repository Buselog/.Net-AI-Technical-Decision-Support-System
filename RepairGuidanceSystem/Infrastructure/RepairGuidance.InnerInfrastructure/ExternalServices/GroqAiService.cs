using Microsoft.Extensions.Configuration;
using RepairGuidance.Application.Dtos;
using RepairGuidance.Application.Managers;
using RepairGuidance.Application.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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

        public async Task<AiRepairResult> GetRepairGuidanceAsync(string userProblem, string deviceName, List<string> availableTools, string targetLevel)
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
                content = @"Sen profesyonel bir tamir asistanısın. Sadece Türkçe cevap ver. 
                            Cevaplarını MUTLAKA VE KESİNLİKLE şu formatta ver: 
                            [Adım X: Talimat Metni | Alet: Alet Adı]
                            Kullanıcının elindeki aletleri önceliklendir. 
                            Eğer elinde yoksa ve kritikse mutlaka gerektiğini belirt.
                            Başka hiçbir açıklama metni ekleme, sadece bu formatta adımları sırala."

            },
                    new {
                        role = "user",
                content = $"Cihaz: {deviceName}. Sorun: {userProblem}. Kullanıcı Seviyesi: {targetLevel}.  Elindeki Aletler: {string.Join(", ", availableTools)}."
            }
        },
                temperature = 0.3
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