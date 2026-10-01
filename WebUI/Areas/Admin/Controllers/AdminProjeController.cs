using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WebUI.Controllers;
using WebUI.Dtos.ProjeDtos;
using WebUI.Dtos.StajBasvuruDtos;
using WebUI.Helpers;

namespace WebUI.Areas.Admin.Controllers;

[Area("Admin")]
public class AdminProjeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseUrl;

    public AdminProjeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Lutfen once sisteme giris yapiniz.";
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        if (!UserPermissionHelper.CanAccessProjeYonetimi(token))
        {
            TempData["ErrorMessage"] = "Proje Yonetimi sayfasina erisim yetkiniz bulunmamaktadir.";
            return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
        }

        var viewModel = new AdminProjeViewModel();

        try
        {
            var projeResponse = await client.GetAsync($"{_baseUrl}/api/Projeler");
            if (projeResponse.IsSuccessStatusCode)
            {
                var json = await projeResponse.Content.ReadAsStringAsync();
                viewModel.Projeler = JsonConvert.DeserializeObject<List<ResultProjeDto>>(json) ?? new List<ResultProjeDto>();
            }

            var basvuruResponse = await client.GetAsync($"{_baseUrl}/api/StajBasvurulari?PageIndex=0&PageSize=200");
            if (basvuruResponse.IsSuccessStatusCode)
            {
                var basvuruJson = await basvuruResponse.Content.ReadAsStringAsync();
                var basvuruResult = JsonConvert.DeserializeObject<ResultStajBasvuruPaginationDto>(basvuruJson);
                if (basvuruResult?.Items != null)
                {
                    viewModel.KabulEdilenOgrenciler = basvuruResult.Items
                        .Where(x => x.StajBasvuruDurumu == 2)
                        .OrderBy(x => x.AdSoyad)
                        .ToList();
                }
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Veriler yuklenirken bir hata olustu: {ex.Message}";
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProjeDto dto)
    {
        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        if (!UserPermissionHelper.CanAccessProjeYonetimi(token))
        {
            TempData["ErrorMessage"] = "Proje olusturma yetkiniz bulunmamaktadir.";
            return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
        }

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Lutfen tum zorunlu alanlari dogru doldurunuz.";
            return RedirectToAction(nameof(Index));
        }

        dto.YoneticiId = LoginController.GetUserIdFromToken(token) ?? 1;

        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{_baseUrl}/api/Projeler", content);
        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Proje basariyla olusturuldu.";
        }
        else
        {
            var err = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Proje olusturulamadi: {err}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateProjeDto dto)
    {
        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        if (!UserPermissionHelper.CanAccessProjeYonetimi(token))
        {
            TempData["ErrorMessage"] = "Proje duzenleme yetkiniz bulunmamaktadir.";
            return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
        }

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Lutfen tum zorunlu alanlari dogru doldurunuz.";
            return RedirectToAction(nameof(Index));
        }

        dto.YoneticiId = LoginController.GetUserIdFromToken(token) ?? 1;

        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PutAsync($"{_baseUrl}/api/Projeler", content);
        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Proje basariyla guncellendi.";
        }
        else
        {
            var err = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Proje guncellenemedi: {err}";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        if (!UserPermissionHelper.CanAccessProjeYonetimi(token))
        {
            TempData["ErrorMessage"] = "Proje silme yetkiniz bulunmamaktadir.";
            return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
        }

        var response = await client.DeleteAsync($"{_baseUrl}/api/Projeler/{id}");
        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Proje basariyla silindi.";
        }
        else
        {
            TempData["ErrorMessage"] = "Proje silinirken hata olustu.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(AssignProjeDto dto)
    {
        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        if (!UserPermissionHelper.CanAccessProjeTakip(token))
        {
            TempData["ErrorMessage"] = "Proje gorevlendirme yetkiniz bulunmamaktadir.";
            return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
        }

        if (dto.ProjeId <= 0 || dto.StajBasvuruId <= 0)
        {
            TempData["ErrorMessage"] = "Gecersiz proje veya ogrenci secimi.";
            return RedirectToAction(nameof(Index));
        }

        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{_baseUrl}/api/Projeler/Assign", content);
        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Proje basariyla stajyere atandi ve ogrenciye bilgilendirme mesaji iletildi.";
            return RedirectToAction(nameof(StajyerProjeleri));
        }
        else
        {
            var err = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Proje atamasi basarisiz oldu: {err}";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> StajyerProjeleri(int? projeId, int? stajBasvuruId)
    {
        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Lutfen once sisteme giris yapiniz.";
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        if (!UserPermissionHelper.CanAccessProjeTakip(token))
        {
            TempData["ErrorMessage"] = "Proje Takip & Notlandirma sayfasina erisim yetkiniz bulunmamaktadir.";
            return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
        }

        List<ResultStajyerProjeDto> list = new();

        try
        {
            var query = new StringBuilder($"{_baseUrl}/api/Projeler/StajyerProjeleri?");
            if (projeId.HasValue) query.Append($"projeId={projeId.Value}&");
            if (stajBasvuruId.HasValue) query.Append($"stajBasvuruId={stajBasvuruId.Value}&");

            var response = await client.GetAsync(query.ToString().TrimEnd('&', '?'));
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<ResultStajyerProjeDto>>(json) ?? new List<ResultStajyerProjeDto>();
            }

            var projelerRes = await client.GetAsync($"{_baseUrl}/api/Projeler");
            if (projelerRes.IsSuccessStatusCode)
            {
                var projelerJson = await projelerRes.Content.ReadAsStringAsync();
                ViewBag.Projeler = JsonConvert.DeserializeObject<List<ResultProjeDto>>(projelerJson) ?? new List<ResultProjeDto>();
            }

            var basvuruRes = await client.GetAsync($"{_baseUrl}/api/StajBasvurulari?PageIndex=0&PageSize=200");
            if (basvuruRes.IsSuccessStatusCode)
            {
                var basvuruJson = await basvuruRes.Content.ReadAsStringAsync();
                var basvuruResult = JsonConvert.DeserializeObject<ResultStajBasvuruPaginationDto>(basvuruJson);
                if (basvuruResult?.Items != null)
                {
                    ViewBag.KabulEdilenOgrenciler = basvuruResult.Items
                        .Where(x => x.StajBasvuruDurumu == 2)
                        .OrderBy(x => x.AdSoyad)
                        .ToList();
                }
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Atamalar yuklenirken hata olustu: {ex.Message}";
        }

        ViewBag.SeciliProjeId = projeId;
        ViewBag.SeciliStajBasvuruId = stajBasvuruId;

        return View(list);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Evaluate(UpdateStajyerProjeDurumDto dto)
    {
        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        if (!UserPermissionHelper.CanAccessProjeTakip(token))
        {
            TempData["ErrorMessage"] = "Proje notlandirma yetkiniz bulunmamaktadir.";
            return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
        }

        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PutAsync($"{_baseUrl}/api/Projeler/StajyerProjeDurum", content);
        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Stajyer projesi degerlendirildi ve durumu guncellendi.";
        }
        else
        {
            TempData["ErrorMessage"] = "Degerlendirme kaydedilirken bir hata olustu.";
        }

        return RedirectToAction(nameof(StajyerProjeleri));
    }
}
