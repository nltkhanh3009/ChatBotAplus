using System.ComponentModel.DataAnnotations;

namespace ChatBotAplus.Models
{
    public class PdfChunk
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string ChunkText { get; set; } = string.Empty;

        public string? EmbeddingJson { get; set; } // chỗ lưu vector embedding (dạng JSON)

    }
}
