using Microsoft.EntityFrameworkCore;
using ChatBotAplus.Models;

namespace ChatBotAplus.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<PdfChunk> PdfChunks { get; set; }
    }
}
