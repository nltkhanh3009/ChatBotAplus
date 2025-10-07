using ChatBotAplus.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<PdfService>();
builder.Services.AddSingleton<EmbeddingService>();
builder.Services.AddSingleton<AiService>();

var app = builder.Build();

// 🔹 Khi app khởi động, load sẵn tất cả PDF trong folder "wwwroot/pdfs"
var pdfService = app.Services.GetRequiredService<PdfService>();
pdfService.LoadAllPdfs("wwwroot/pdfs");

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Chat}/{action=Index}/{id?}");

app.Run();
