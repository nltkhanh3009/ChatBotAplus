using Microsoft.AspNetCore.Mvc;
using ChatBotAplus.Services;
using Markdig;

namespace ChatBotAplus.Controllers
{
    public class ChatController : Controller
    {
        private readonly PdfService _pdfService;
        private readonly EmbeddingService _embedService;
        private readonly AiService _aiService;

        public ChatController(PdfService pdfService, EmbeddingService embedService, AiService aiService)
        {
            _pdfService = pdfService;
            _embedService = embedService;
            _aiService = aiService;
        }

        public IActionResult Index() => View();

        [HttpPost]
        // Lưu ý: Tên đối số nên khớp với tên trường trong model gửi từ JS (hoặc dùng [FromForm])
        public async Task<IActionResult> Ask([FromForm] string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return Json(new { answer = "Vui lòng nhập câu hỏi." });

            // Thêm logic kiểm tra lời chào đơn giản để tiết kiệm API call
            var userQuestionLower = question.Trim().ToLower();
            if (userQuestionLower == "hi" || userQuestionLower == "chào" || userQuestionLower == "hello")
            {
                return Json(new { answer = "Bạn cần mình tư vấn gì về vấn đề của Aplus?" });
            }

            try
            {
                // Chọn đoạn tài liệu liên quan nhất (Logic RAG thủ công)
                var qEmbed = await _embedService.GetEmbeddingAsync(question);
                string bestText = "";
                double bestScore = 0;

                // LƯU Ý: Đây là phương pháp RAG KHÔNG HIỆU QUẢ 
                // vì nó tạo embedding cho TOÀN BỘ tài liệu *mỗi lần* người dùng hỏi.
                foreach (var doc in _pdfService.AllDocuments)
                {
                    var e = await _embedService.GetEmbeddingAsync(doc.Text.Substring(0, Math.Min(1000, doc.Text.Length)));
                    double score = _embedService.CosineSimilarity(qEmbed, e);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestText = doc.Text;
                    }
                }

                // === BƯỚC SỬA LỖI 1: Sửa thứ tự đối số trong AskGeminiAsync ===
                // Hàm AskGeminiAsync nhận (question, context), nhưng bạn đang truyền (context, question)
                var markdownAnswer = await _aiService.AskGeminiAsync(question, bestText);

                // === BƯỚC SỬA LỖI 2: CHUYỂN ĐỔI MARKDOWN SANG HTML ===
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                var htmlAnswer = Markdig.Markdown.ToHtml(markdownAnswer, pipeline);

                return Json(new { answer = htmlAnswer });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (ex) ở đây
                return Json(new { answer = "Xin lỗi, đã xảy ra lỗi khi xử lý câu hỏi của bạn." });
            }
        }
    }
}
