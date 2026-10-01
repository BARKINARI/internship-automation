using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FilesController : BaseController
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public FilesController(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpPost("UploadCv")]
    public async Task<IActionResult> UploadCv(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Lütfen geçerli bir dosya seçiniz.");

        string[] allowedExtensions = [".pdf", ".doc", ".docx", ".png", ".jpg", ".jpeg"];
        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            return BadRequest("Sadece PDF, DOC, DOCX, PNG veya JPG formatında dosya yükleyebilirsiniz.");

        if (file.Length > 10 * 1024 * 1024)
            return BadRequest("Dosya boyutu en fazla 10MB olabilir.");

        try
        {
            string webRootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string uploadsFolder = Path.Combine(webRootPath, "uploads", "cvs");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }

            string fileUrl = $"/uploads/cvs/{uniqueFileName}";
            return Ok(new { FilePath = fileUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Dosya kaydedilirken sunucu hatası oluştu: {ex.Message}");
        }
    }

    [HttpPost("UploadMesajDosya")]
    [RequestSizeLimit(52428800)]
    public async Task<IActionResult> UploadMesajDosya(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Lütfen geçerli bir dosya seçiniz.");

        string[] allowedExtensions = [
            ".zip", ".rar", ".7z", ".tar", ".gz",
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt",
            ".png", ".jpg", ".jpeg", ".gif", ".webp",
            ".sql", ".cs", ".py", ".ts", ".css", ".json", ".xml", ".csv"
        ];
        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            return BadRequest("Desteklenmeyen dosya formatı. (Desteklenenler: zip, rar, 7z, pdf, docx, xlsx, txt, görseller, sql, cs, py, json vb.)");

        if (file.Length > 50 * 1024 * 1024)
            return BadRequest("Dosya boyutu en fazla 50MB olabilir.");

        try
        {
            string webRootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string uploadsFolder = Path.Combine(webRootPath, "uploads", "mesajlar");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }

            string fileUrl = $"/uploads/mesajlar/{uniqueFileName}";
            return Ok(new { FilePath = fileUrl, FileName = file.FileName, FileSize = file.Length });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Mesaj dosyası kaydedilirken hata oluştu: {ex.Message}");
        }
    }

    [HttpDelete("DeleteCv")]
    public IActionResult DeleteCv([FromQuery] string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return BadRequest("Dosya yolu belirtilmelidir.");

        try
        {
            string webRootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string cvsDirectory = Path.GetFullPath(Path.Combine(webRootPath, "uploads", "cvs"));

            string fileName = Path.GetFileName(filePath);
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("Geçersiz dosya adı.");

            string fullPath = Path.GetFullPath(Path.Combine(cvsDirectory, fileName));

            if (!fullPath.StartsWith(cvsDirectory, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Geçersiz dosya yolu.");

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                return Ok(new { Message = "Dosya başarıyla silindi." });
            }

            return NotFound("Dosya bulunamadı.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Dosya silinirken hata oluştu: {ex.Message}");
        }
    }
}