// Services/IRAGService.cs

using System.Threading.Tasks;

namespace ChatBotAplus.Services
{
    public interface IRAGService
    {
        /// <summary>
        /// Xử lý câu hỏi, tìm context từ Vector DB và gọi AI để tạo câu trả lời.
        /// </summary>
        /// <param name="question">Câu hỏi từ người dùng.</param>
        /// <returns>Câu trả lời dưới dạng HTML.</returns>
        Task<string> GetAnswerFromPdfContextAsync(string question);
    }
}