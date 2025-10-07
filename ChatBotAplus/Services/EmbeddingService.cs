using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace ChatBotAplus.Services
{
    public class EmbeddingService
    {
        // Sử dụng một tên biến hằng (const) hoặc lấy từ cấu hình (Configuration)
        private const string ApiKey = "AIzaSyCD-TYHhaO0KPjNdsiTKySIjBG5HxeM3uQ"; // 🔑 Thay bằng key thật

        public async Task<List<float>> GetEmbeddingAsync(string text)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/text-embedding-004:embedContent?key={ApiKey}";
            var client = new RestClient(url);
            var request = new RestRequest("", Method.Post);
            request.AddHeader("Content-Type", "application/json");

            // *** SỬA LỖI 1: CHỈNH SỬA CẤU TRÚC REQUEST BODY ***
            var requestBody = new
            {
                content = new
                {
                    parts = new[]
                    {
                        new { text = text }
                    }
                }
            };
            request.AddJsonBody(requestBody);

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                // Cải thiện xử lý lỗi để in ra chi tiết phản hồi từ server
                var errorDetail = response.Content;
                if (string.IsNullOrEmpty(errorDetail))
                {
                    errorDetail = $"Status Code: {response.StatusCode}. {response.ErrorMessage}";
                }
                throw new Exception($"Gemini Embedding API error: {errorDetail}");
            }

            var json = JObject.Parse(response.Content!);

            // *** SỬA LỖI 2: CHỈNH SỬA TRUY VẤN JSON ***
            // Vị trí chính xác của vector là json["embedding"]["values"]
            var embeddingValues = json["embedding"]?["values"];

            if (embeddingValues == null)
            {
                throw new Exception($"Lỗi: Không tìm thấy mảng 'values' trong phản hồi. Phản hồi thô: {response.Content}");
            }

            return embeddingValues.Select(v => (float)v).ToList();
        }

        public double CosineSimilarity(List<float> a, List<float> b)
        {
            // Logic tính Cosine Similarity là chính xác
            if (a.Count != b.Count) return 0;
            double dot = 0, normA = 0, normB = 0;
            for (int i = 0; i < a.Count; i++)
            {
                dot += a[i] * b[i];
                normA += a[i] * a[i];
                normB += b[i] * b[i];
            }
            // Cộng 1e-10 để tránh chia cho 0
            return dot / (Math.Sqrt(normA) * Math.Sqrt(normB) + 1e-10);
        }
    }
}