using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AquarSmartCourt.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Lấy ApiKey từ file appsettings.json
            _apiKey = configuration["Gemini:ApiKey"]?.Trim();
        }

        public async Task<string> GetChatResponseAsync(string userMessage)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return "Cấu hình API Key cho Gemini AI bị thiếu trong file appsettings.json!";
            }

            try
            {
                // Bộ đôi chuẩn: v1beta ĐI KÈM VỚI gemini-2.0-flash
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new { parts = new[] { new { text = userMessage } } }
                    }
                };

                var jsonPayload = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return $"Google API báo lỗi (Mã {response.StatusCode}): {responseString}";
                }

                // Đọc dữ liệu JSON trả về
                using var doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;

                if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                {
                    var firstCandidate = candidates[0];
                    if (firstCandidate.TryGetProperty("content", out var resContent))
                    {
                        if (resContent.TryGetProperty("parts", out var parts) && parts.GetArrayLength() > 0)
                        {
                            var replyText = parts[0].GetProperty("text").GetString();
                            return replyText ?? "AI phản hồi trống.";
                        }
                    }
                }

                return "Không thể phân tích nội dung phản hồi từ AI.";
            }
            catch (Exception ex)
            {
                return $"Đã xảy ra sự cố hệ thống khi kết nối AI: {ex.Message}";
            }
        }
    }
}