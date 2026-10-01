using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WebUI.Dtos.StajBasvuruDtos;
using WebUI.Helpers;

namespace WebUI.Areas.Admin.Controllers;

[Area("Admin")]
public class AdminStajBasvuruController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseUrl;

    public AdminStajBasvuruController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _baseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://localhost:7125";
    }

    private HttpClient CreateApiClient(out string? token)
    {
        var client = _httpClientFactory.CreateClient();
        token = Request.Cookies["access_token"];
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return client;
    }

    private IActionResult HandleUnauthorized(string? errorContent = null)
    {
        string msg = "Bu islemi gerceklestirmek icin yetkiniz bulunmamaktadir.";
        if (!string.IsNullOrEmpty(errorContent))
        {
            try
            {
                dynamic? errObj = JsonConvert.DeserializeObject(errorContent);
                if (errObj != null && errObj.detail != null)
                {
                    msg = errObj.detail.ToString();
                }
            }
            catch { }
        }

        TempData["ErrorMessage"] = msg;
        var token = Request.Cookies["access_token"];
        return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
    }

    [HttpGet]
    public async Task<IActionResult> StajBasvuruList()
    {
        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Lutfen once sisteme giris yapiniz.";
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        if (!UserPermissionHelper.CanAccessBasvurular(token))
        {
            TempData["ErrorMessage"] = "Staj Basvurulari sayfasina erisim yetkiniz bulunmamaktadir.";
            return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
        }

        try
        {
            var responseMessage = await client.GetAsync($"{_baseUrl}/api/StajBasvurulari?PageIndex=0&PageSize=100&bypassCache=true");
            
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized || responseMessage.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                var errContent = await responseMessage.Content.ReadAsStringAsync();
                return HandleUnauthorized(errContent);
            }

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResultStajBasvuruPaginationDto>(jsonData);
                return View(result?.Items ?? new List<ResultStajBasvuruDto>());
            }
            else
            {
                var errContent = await responseMessage.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = $"WebAPI'den yanit alinamadi. (Durum: {(int)responseMessage.StatusCode} - {responseMessage.ReasonPhrase}): {errContent}";
            }
        }
        catch (HttpRequestException ex)
        {
            ViewBag.ErrorMessage = $"WebAPI sunucusuna ({_baseUrl}) baglanilamadi. Lutfen WebAPI projesinin acik oldugundan emin olunuz. ({ex.Message})";
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"Beklenmeyen bir hata olustu: {ex.Message}";
        }

        return View(new List<ResultStajBasvuruDto>());
    }

    [HttpGet]
    public IActionResult CreateStajBasvuru()
    {
        var token = Request.Cookies["access_token"];
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Lutfen once sisteme giris yapiniz.";
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        if (!UserPermissionHelper.CanAccessYeniBasvuru(token))
        {
            TempData["ErrorMessage"] = "Yeni Basvuru Yap sayfasina erisim yetkiniz bulunmamaktadir.";
            return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
        }

        return View(new CreateStajBasvuruDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateStajBasvuru(CreateStajBasvuruDto createStajBasvuruDto, IFormFile? cvFile)
    {
        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Lutfen once sisteme giris yapiniz.";
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        if (!UserPermissionHelper.CanAccessYeniBasvuru(token))
        {
            TempData["ErrorMessage"] = "Yeni Basvuru Yap sayfasina erisim yetkiniz bulunmamaktadir.";
            return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
        }

        try
        {
            if (cvFile != null && cvFile.Length > 0)
            {
                try
                {
                    using var content = new MultipartFormDataContent();
                    using var fileStream = cvFile.OpenReadStream();
                    using var streamContent = new StreamContent(fileStream);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(cvFile.ContentType ?? "application/pdf");
                    content.Add(streamContent, "file", cvFile.FileName);

                    var uploadResponse = await client.PostAsync($"{_baseUrl}/api/Files/UploadCv", content);
                    if (uploadResponse.IsSuccessStatusCode)
                    {
                        var uploadJson = await uploadResponse.Content.ReadAsStringAsync();
                        dynamic? uploadResult = JsonConvert.DeserializeObject(uploadJson);
                        if (uploadResult != null && uploadResult.filePath != null)
                        {
                            createStajBasvuruDto.CvDosyaYolu = uploadResult.filePath.ToString();
                        }
                    }
                }
                catch (Exception uploadEx)
                {
                    ModelState.AddModelError(string.Empty, $"CV yuklenirken uyari: {uploadEx.Message}");
                }
            }

            var jsonData = JsonConvert.SerializeObject(createStajBasvuruDto);
            var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync($"{_baseUrl}/api/StajBasvurulari", stringContent);

            if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized || responseMessage.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                var errContent = await responseMessage.Content.ReadAsStringAsync();
                return HandleUnauthorized(errContent);
            }

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Staj basvurusu basariyla kaydedildi!";
                return RedirectToAction("StajBasvuruList", "AdminStajBasvuru", new { area = "Admin" });
            }

            var errorBody = await responseMessage.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Basvuru kaydedilemedi: {errorBody}");
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, $"WebAPI sunucusuna ({_baseUrl}) baglanilamadi. Lutfen WebAPI projesinin acik oldugundan emin olunuz. ({ex.Message})");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Bir hata olustu: {ex.Message}");
        }

        return View(createStajBasvuruDto);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteStajBasvuru(int id)
    {
        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Lutfen once sisteme giris yapiniz.";
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        if (!UserPermissionHelper.CanAccessBasvurular(token))
        {
            TempData["ErrorMessage"] = "Staj Basvurulari silme yetkiniz bulunmamaktadir.";
            return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
        }

        try
        {
            var responseMessage = await client.DeleteAsync($"{_baseUrl}/api/StajBasvurulari/{id}");
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized || responseMessage.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                var errContent = await responseMessage.Content.ReadAsStringAsync();
                return HandleUnauthorized(errContent);
            }

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Staj basvurusu basariyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Silme islemi basarisiz oldu.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Silme islemi basarisiz: {ex.Message}";
        }

        return RedirectToAction("StajBasvuruList", "AdminStajBasvuru", new { area = "Admin" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveAndSendPassword(int id, string password)
    {
        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Lutfen once sisteme giris yapiniz.";
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        if (!UserPermissionHelper.CanAccessBasvurular(token))
        {
            TempData["ErrorMessage"] = "Staj Basvurulari onaylama yetkiniz bulunmamaktadir.";
            return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
        }

        if (string.IsNullOrWhiteSpace(password) || password.Trim().Length < 6)
        {
            TempData["ErrorMessage"] = "Öğrenciye atanacak şifre en az 6 karakter olmalıdır.";
            return RedirectToAction("StajBasvuruList", "AdminStajBasvuru", new { area = "Admin" });
        }

        try
        {
            var command = new
            {
                StajBasvuruId = id,
                Password = password.Trim()
            };

            var jsonData = JsonConvert.SerializeObject(command);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var responseMessage = await client.PostAsync($"{_baseUrl}/api/StajBasvurulari/ApproveAndSendPassword", content);

            if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized || responseMessage.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                var errContent = await responseMessage.Content.ReadAsStringAsync();
                return HandleUnauthorized(errContent);
            }

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Stajyer başvurusu ONAYLANDI, öğrenci hesabı hazırlandı ve şifresi e-posta adresine başarıyla gönderildi!";
            }
            else
            {
                var errContent = await responseMessage.Content.ReadAsStringAsync();
                try
                {
                    dynamic? errObj = JsonConvert.DeserializeObject(errContent);
                    if (errObj?.Errors != null && errObj.Errors.Count > 0 && errObj.Errors[0].Errors != null && errObj.Errors[0].Errors.Count > 0)
                    {
                        TempData["ErrorMessage"] = errObj.Errors[0].Errors[0].ToString();
                    }
                    else if (errObj?.detail != null)
                    {
                        TempData["ErrorMessage"] = errObj.detail.ToString();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Onaylama ve şifre gönderme işlemi başarısız oldu.";
                    }
                }
                catch
                {
                    TempData["ErrorMessage"] = "Onaylama ve şifre gönderme işlemi başarısız oldu.";
                }
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Islem sirasinda hata: {ex.Message}";
        }

        return RedirectToAction("StajBasvuruList", "AdminStajBasvuru", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> ChangeStatus(int id, int status)
    {
        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Lutfen once sisteme giris yapiniz.";
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        if (!UserPermissionHelper.CanAccessBasvurular(token))
        {
            TempData["ErrorMessage"] = "Staj Basvuru durumunu degistirme yetkiniz bulunmamaktadir.";
            return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
        }

        try
        {
            var getResponse = await client.GetAsync($"{_baseUrl}/api/StajBasvurulari/{id}");
            if (getResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized || getResponse.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                var errContent = await getResponse.Content.ReadAsStringAsync();
                return HandleUnauthorized(errContent);
            }

            if (getResponse.IsSuccessStatusCode)
            {
                var jsonData = await getResponse.Content.ReadAsStringAsync();
                var basvuru = JsonConvert.DeserializeObject<UpdateStajBasvuruDto>(jsonData);
                if (basvuru != null)
                {
                    basvuru.StajBasvuruDurumu = status;
                    var updateJson = JsonConvert.SerializeObject(basvuru);
                    var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");
                    var updateResponse = await client.PutAsync($"{_baseUrl}/api/StajBasvurulari", updateContent);
                    if (updateResponse.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Basvuru durumu basariyla guncellendi.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Durum guncellenemedi.";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Durum guncellenirken hata: {ex.Message}";
        }

        return RedirectToAction("StajBasvuruList", "AdminStajBasvuru", new { area = "Admin" });
    }
}
