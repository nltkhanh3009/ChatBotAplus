namespace ChatBotAplus.Models
{
    public class ChunkEmbedding
    {
        public string Text { get; set; } = "";
        public List<float> Vector { get; set; } = new();
    }
}
