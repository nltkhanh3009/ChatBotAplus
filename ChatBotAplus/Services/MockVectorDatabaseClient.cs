// Services/MockVectorDatabaseClient.cs

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBotAplus.Services
{
    // Lớp này triển khai IVectorDatabaseClient (Đã được tạo ở bước trước)
    public class MockVectorDatabaseClient : IVectorDatabaseClient
    {
        // Dữ liệu giả lập mô phỏng các đoạn text (chunks) từ PDF
        private readonly List<string> MockContexts = new List<string>
        {
            "Tên sản phẩm: Aplus Pro X, Giá bán: 15,000,000 VND, Link sản phẩm: https://aplus.com/pro-x",
            "Tên sản phẩm: Aplus Eco-Gen, Giá bán: 8,500,000 VND, Link sản phẩm: https://aplus.com/eco-gen",
            "Chính sách hỗ trợ kỹ thuật: Vui lòng gửi email đến support@aplus.com trong giờ hành chính.",
            "Phí vận chuyển cho đơn hàng dưới 10 triệu đồng là 50,000 VND, miễn phí cho đơn hàng lớn hơn.",
            "Tên sản phẩm: Aplus Ultra-View 55, Giá bán: 22,900,000 VND, Link sản phẩm: https://aplus.com/ultra-view"
        };

        /// <summary>
        /// Mô phỏng việc tìm kiếm context liên quan dựa trên vector (embedding).
        /// </summary>
        public Task<List<RetrievalResult>> SearchAsync(List<float> embedding, int topK)
        {
            // Trong môi trường thực: Bạn sẽ dùng vector 'embedding' để truy vấn DB.
            // Ở đây, ta chỉ giả lập trả về topK đoạn context đầu tiên và gán Score giả lập.

            var results = MockContexts
                .Take(topK)
                .Select((text, index) => new RetrievalResult
                {
                    Text = text,
                    // Giả lập điểm số tương đồng cao (0.9, 0.8, 0.7...)
                    Score = 0.9f - (index * 0.1f)
                })
                .ToList();

            return Task.FromResult(results);
        }
    }
}