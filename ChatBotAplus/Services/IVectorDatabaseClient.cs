// Services/IVectorDatabaseClient.cs

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBotAplus.Services
{
    // DTO để chứa kết quả tìm kiếm
    public class RetrievalResult
    {
        public string Text { get; set; } // Đoạn văn bản (Context) được trích xuất
        public float Score { get; set; }
    }

    public interface IVectorDatabaseClient
    {
        Task<List<RetrievalResult>> SearchAsync(List<float> embedding, int topK);
        // Task IndexDataAsync(Dictionary<string, List<float>> dataToStore); // Dành cho logic Indexing
    }
}