using Microsoft.AspNetCore.Mvc;
using ChatBotAplus.Services;

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
        public async Task<IActionResult> Ask(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return Json(new { answer = "Vui lòng nhập câu hỏi." });

            // Chọn đoạn tài liệu liên quan nhất
            var qEmbed = await _embedService.GetEmbeddingAsync(question);
            string bestText = "";
            double bestScore = 0;

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

            var answer = await _aiService.AskGeminiAsync(bestText, question);
            return Json(new { answer });
        }
    }
}
