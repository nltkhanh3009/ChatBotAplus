using RestSharp;
using Newtonsoft.Json.Linq;

namespace ChatBotAplus.Services
{
    public class AiService
    {
        // Khuyến nghị: Đọc API Key từ IConfiguration thay vì hardcode
        private readonly string apiKey = "AIzaSyCD-TYHhaO0KPjNdsiTKySIjBG5HxeM3uQ";

        public async Task<string> AskGeminiAsync(string question, string context)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";
            var client = new RestClient(url);
            var request = new RestRequest("", Method.Post);
            request.AddHeader("Content-Type", "application/json");

            // Cải tiến prompt với HƯỚNG DẪN HỆ THỐNG rõ ràng
            var systemInstruction = "Bạn là trợ lý AI chuyên nghiệp, thân thiện cho Aplus. Hãy trả lời câu hỏi của người dùng DỰA DUY NHẤT VÀO TÀI LIỆU được cung cấp,chỉ đưa ra thông tin Tên sản phẩm, giá bán và link dẫn dến sản phẩm,- nếu người dùng cần thì trả lời thêm vấn đề đó, nhưng đừng nhắc gì đến tài liệu." +
                " Nếu tài liệu không chứa câu trả lời, hãy trả lời lịch sự rằng bạn không có đủ thông tin.";
            var fullPrompt = $"{systemInstruction}\n\n[TÀI LIỆU]: {context}\n\n[CÂU HỎI]: {question}";

            var body = new
            {
                contents = new[]
                {
                    new {
                        parts = new[] {
                            new { text = fullPrompt }
                        }
                    }
                }
            };

            request.AddJsonBody(body);
            var response = await client.ExecuteAsync(request);

            // Xử lý lỗi API
            if (!response.IsSuccessful)
            {
                var errorDetail = response.Content;
                if (string.IsNullOrEmpty(errorDetail))
                {
                    errorDetail = $"Status Code: {response.StatusCode}. {response.ErrorMessage}";
                }
                throw new Exception($"Gemini API Generation error: {errorDetail}");
            }

            // Phân tích JSON và trích xuất câu trả lời
            var json = JObject.Parse(response.Content!);

            // Cấu trúc truy vấn JSON có vẻ chính xác cho phản hồi Gemini
            return json["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString()
                    ?? "Xin lỗi, tôi không tìm thấy thông tin phù hợp.";
        }
    }
}