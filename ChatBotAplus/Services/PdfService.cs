using UglyToad.PdfPig;
using System.Text;

namespace ChatBotAplus.Services
{
    public class PdfService
    {
        public List<(string FileName, string Text)> AllDocuments { get; private set; } = new();

        public void LoadAllPdfs(string folderPath)
        {
            if (!Directory.Exists(folderPath)) return;

            AllDocuments.Clear();

            foreach (var file in Directory.GetFiles(folderPath, "*.pdf"))
            {
                string text = ExtractText(file);
                AllDocuments.Add((Path.GetFileName(file), text));
            }
        }

        private string ExtractText(string filePath)
        {
            var sb = new StringBuilder();
            using (var pdf = PdfDocument.Open(filePath))
            {
                foreach (var page in pdf.GetPages())
                    sb.AppendLine(page.Text);
            }
            return sb.ToString();
        }
    }
}
