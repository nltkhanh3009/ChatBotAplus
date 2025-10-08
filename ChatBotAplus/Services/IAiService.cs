namespace ChatBotAplus.Services
{
    public interface IAiService
    {
        Task<string> AskGeminiAsync(string question, string context);
    }
}
