// Services/RAGService.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Markdig; // Cần cài NuGet Markdig

namespace ChatBotAplus.Services
{
    public class RAGService : IRAGService
    {
        // Phụ thuộc vào các dịch vụ khác
        private readonly EmbeddingService _embeddingService; // Dịch vụ nhúng
        private readonly IVectorDatabaseClient _dbClient;      // Dịch vụ Vector DB
        private readonly AiService _aiService;               // Dịch vụ gọi Gemini (Generation)

        // Cấu trúc: [I]AiService, [I]VectorDatabaseClient, EmbeddingService
        public RAGService(
            EmbeddingService embeddingService,
            IVectorDatabaseClient dbClient,
            AiService aiService)
        {
            _embeddingService = embeddingService;
            _dbClient = dbClient;
            _aiService = aiService;
        }

        public async Task<string> GetAnswerFromPdfContextAsync(string question)
        {
            // === BƯỚC 1: EMBEDDING (Nhúng câu hỏi) ===
            var questionVector = await _embeddingService.GetEmbeddingAsync(question);

            // === BƯỚC 2: RETRIEVAL (Tìm Context liên quan) ===
            // Tìm 3 đoạn văn bản liên quan nhất từ Vector DB
            var searchResults = await _dbClient.SearchAsync(questionVector, topK: 3);

            // Nối các đoạn context thành một chuỗi duy nhất
            var context = string.Join("\n---\n", searchResults.Select(r => r.Text));

            // === BƯỚC 3: GENERATION (Gọi AI tạo câu trả lời) ===
            // Lấy câu trả lời bằng Markdown từ Gemini
            var markdownAnswer = await _aiService.AskGeminiAsync(question, context);

            // === BƯỚC 4: CHUYỂN ĐỔI (MARKDOWN TO HTML) ===
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var htmlAnswer = Markdown.ToHtml(markdownAnswer, pipeline);

            return htmlAnswer;
        }
    }
}