using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using WebUI.Dtos.StajBasvuruDtos;
using WebUI.Models;

namespace WebUI.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseUrl;

    public HomeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _baseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://localhost:7125";
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new CreateStajBasvuruDto
        {
            StajBaslangicTarihi = DateTime.Today.AddDays(7),
            StajBitisTarihi = DateTime.Today.AddDays(37)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CreateStajBasvuruDto createStajBasvuruDto, IFormFile? cvFile)
    {
        var client = _httpClientFactory.CreateClient();

        if (cvFile != null && cvFile.Length > 0)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var fileStream = cvFile.OpenReadStream();
                using var streamContent = new StreamContent(fileStream);
                
                string extension = Path.GetExtension(cvFile.FileName);
                string safeUploadFileName = $"cv_{Guid.NewGuid():N}{extension}";

                if (!string.IsNullOrEmpty(cvFile.ContentType) && MediaTypeHeaderValue.TryParse(cvFile.ContentType, out var parsedContentType))
                    streamContent.Headers.ContentType = parsedContentType;
                else
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                content.Add(streamContent, "file", safeUploadFileName);

                var uploadResponse = await client.PostAsync($"{_baseUrl}/api/Files/UploadCv", content);
                if (uploadResponse.IsSuccessStatusCode)
                {
                    var uploadJson = await uploadResponse.Content.ReadAsStringAsync();
                    var jObj = JObject.Parse(uploadJson);
                    string? uploadedPath = (string?)jObj["filePath"] ?? (string?)jObj["FilePath"];
                    if (!string.IsNullOrEmpty(uploadedPath))
                    {
                        createStajBasvuruDto.CvDosyaYolu = uploadedPath;
                    }
                }
                else
                {
                    var errorMsg = await uploadResponse.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = $"CV Dosyası yüklenemedi: {errorMsg}";
                    return View(createStajBasvuruDto);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"CV yüklenirken bir hata oluştu: {ex.Message}";
                return View(createStajBasvuruDto);
            }
        }

        createStajBasvuruDto.StajBasvuruDurumu = 1; 
        createStajBasvuruDto.StajBaslangicTarihi = DateTime.SpecifyKind(createStajBasvuruDto.StajBaslangicTarihi, DateTimeKind.Utc);
        createStajBasvuruDto.StajBitisTarihi = DateTime.SpecifyKind(createStajBasvuruDto.StajBitisTarihi, DateTimeKind.Utc);

        var jsonData = JsonConvert.SerializeObject(createStajBasvuruDto);
        var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

        try
        {
            var responseMessage = await client.PostAsync($"{_baseUrl}/api/StajBasvurulari", stringContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Tebrikler! Staj başvurunuz ve özgeçmişiniz başarıyla alınmıştır. İnceleme sürecinden sonra sizinle iletişime geçilecektir.";
                return RedirectToAction("Index");
            }

            var errorContent = await responseMessage.Content.ReadAsStringAsync();
            string errorMessage = "Başvuru kaydedilemedi. Lütfen tüm alanları kontrol ediniz.";

            try
            {
                var jObj = JObject.Parse(errorContent);
                if (jObj["detail"] != null)
                    errorMessage = jObj["detail"]!.ToString();
                else if (jObj["title"] != null)
                    errorMessage = jObj["title"]!.ToString();
            }
            catch { }

            ViewBag.ErrorMessage = errorMessage;
            return View(createStajBasvuruDto);
        }
        catch (HttpRequestException)
        {
            ViewBag.ErrorMessage = $"WebAPI servisine ({_baseUrl}) bağlanılamadı! Lütfen WebAPI projesinin çalıştığından emin olunuz.";
            return View(createStajBasvuruDto);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Bir hata oluştu: " + ex.Message;
            return View(createStajBasvuruDto);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}