using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WebUI.Controllers;
using WebUI.Dtos.MesajDtos;
using WebUI.Dtos.ProjeDtos;
using WebUI.Dtos.StajBasvuruDtos;
using WebUI.Helpers;

namespace WebUI.Areas.Admin.Controllers;

[Area("Admin")]
public class AdminMesajController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseUrl;

    public AdminMesajController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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
    public async Task<IActionResult> Index(int? stajBasvuruId, int? projeId)
    {
        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Lutfen once sisteme giris yapiniz.";
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        if (!UserPermissionHelper.CanAccessMesajlar(token))
        {
            TempData["ErrorMessage"] = "Ogrenci Mesajlari sayfasina erisim yetkiniz bulunmamaktadir.";
            return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
        }

        var viewModel = new AdminMesajViewModel();
        int adminId = LoginController.GetUserIdFromToken(token) ?? 1;
        ViewBag.AdminId = adminId;

        try
        {
            var response = await client.GetAsync($"{_baseUrl}/api/StajBasvurulari?PageIndex=0&PageSize=200&bypassCache=true");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResultStajBasvuruPaginationDto>(json);
                if (result?.Items != null)
                {
                    viewModel.KabulEdilenOgrenciler = result.Items
                        .Where(x => x.StajBasvuruDurumu == 2)
                        .OrderByDescending(x => x.Id)
                        .ToList();
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                TempData["ErrorMessage"] = "Yetkiniz bulunmamaktadir.";
                return Redirect(UserPermissionHelper.GetDefaultLandingUrl(token));
            }

            var projelerRes = await client.GetAsync($"{_baseUrl}/api/Projeler/StajyerProjeleri");
            if (projelerRes.IsSuccessStatusCode)
            {
                var projelerJson = await projelerRes.Content.ReadAsStringAsync();
                viewModel.TumStajyerProjeleri = JsonConvert.DeserializeObject<List<ResultStajyerProjeDto>>(projelerJson) ?? new List<ResultStajyerProjeDto>();
            }

            var unreadRes = await client.GetAsync($"{_baseUrl}/api/Mesajlar/UnreadCounts?aliciId={adminId}");
            if (unreadRes.IsSuccessStatusCode)
            {
                var unreadJson = await unreadRes.Content.ReadAsStringAsync();
                viewModel.OkunmamisMesajSayilari = JsonConvert.DeserializeObject<Dictionary<int, int>>(unreadJson) ?? new Dictionary<int, int>();
            }

            foreach (var ogrenci in viewModel.KabulEdilenOgrenciler)
            {
                var ogrenciProjeleri = viewModel.TumStajyerProjeleri
                    .Where(p => p.StajBasvuruId == ogrenci.Id)
                    .ToList();

                if (ogrenciProjeleri.Any())
                {
                    foreach (var pItem in ogrenciProjeleri)
                    {
                        viewModel.ProjeSohbetListesi.Add(new StudentProjectChatItem
                        {
                            StajBasvuruId = ogrenci.Id,
                            OgrenciAdSoyad = $"{ogrenci.Ad} {ogrenci.Soyad}",
                            Okul = ogrenci.Okul,
                            StajKonusu = ogrenci.StajKonusu,
                            Eposta = ogrenci.Eposta,
                            Telefon = ogrenci.Telefon,
                            ProjeId = pItem.ProjeId,
                            ProjeBaslik = pItem.ProjeBaslik,
                            StajyerProjeDurumu = pItem.Durum,
                            DurumMetni = pItem.DurumMetni,
                            DurumRozet = pItem.DurumRozet,
                            UnreadCount = viewModel.OkunmamisMesajSayilari.TryGetValue(ogrenci.Id, out int cnt) ? cnt : 0
                        });
                    }
                }
            }

            if (stajBasvuruId.HasValue && stajBasvuruId.Value > 0 && viewModel.KabulEdilenOgrenciler.Any())
            {
                viewModel.SeciliOgrenci = viewModel.KabulEdilenOgrenciler.FirstOrDefault(x => x.Id == stajBasvuruId.Value);

                if (viewModel.SeciliOgrenci != null)
                {
                    viewModel.SeciliOgrencininProjeleri = viewModel.TumStajyerProjeleri
                        .Where(p => p.StajBasvuruId == viewModel.SeciliOgrenci.Id)
                        .ToList();

                    if (projeId.HasValue)
                    {
                        viewModel.SeciliProjeId = projeId.Value;
                        viewModel.SeciliProje = viewModel.SeciliOgrencininProjeleri.FirstOrDefault(p => p.ProjeId == projeId.Value);
                    }
                    else if (viewModel.SeciliOgrencininProjeleri.Any())
                    {
                        viewModel.SeciliProje = viewModel.SeciliOgrencininProjeleri.First();
                        viewModel.SeciliProjeId = viewModel.SeciliProje.ProjeId;
                    }

                    if (viewModel.SeciliProjeId.HasValue && viewModel.SeciliProjeId.Value > 0)
                    {
                        string msgUrl = $"{_baseUrl}/api/Mesajlar/Konusma/{viewModel.SeciliOgrenci.Id}?projeId={viewModel.SeciliProjeId.Value}&okuyanUserId={adminId}";
                        var msgResponse = await client.GetAsync(msgUrl);
                        if (msgResponse.IsSuccessStatusCode)
                        {
                            var msgJson = await msgResponse.Content.ReadAsStringAsync();
                            viewModel.Mesajlar = JsonConvert.DeserializeObject<List<ResultMesajDto>>(msgJson) ?? new List<ResultMesajDto>();
                        }
                    }

                    if (viewModel.OkunmamisMesajSayilari.ContainsKey(viewModel.SeciliOgrenci.Id))
                    {
                        viewModel.OkunmamisMesajSayilari.Remove(viewModel.SeciliOgrenci.Id);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"Sunucu ile baglanti kurulamadi: {ex.Message}";
        }

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UploadMesajDosya(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Json(new { success = false, message = "Lutfen gecerli bir dosya seciniz." });

        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token))
        {
            return Json(new { success = false, message = "Oturum sureniz dolmus, lutfen yeniden giris yapin." });
        }

        if (!UserPermissionHelper.CanAccessMesajlar(token))
        {
            return Json(new { success = false, message = "Yetkiniz bulunmamaktadir." });
        }

        try
        {
            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            using var streamContent = new StreamContent(stream);

            if (!string.IsNullOrEmpty(file.ContentType) && MediaTypeHeaderValue.TryParse(file.ContentType, out var parsedContentType))
                streamContent.Headers.ContentType = parsedContentType;
            else
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            content.Add(streamContent, "file", file.FileName);

            var uploadResponse = await client.PostAsync($"{_baseUrl}/api/Files/UploadMesajDosya", content);
            if (uploadResponse.IsSuccessStatusCode)
            {
                var uploadJson = await uploadResponse.Content.ReadAsStringAsync();
                var jObj = Newtonsoft.Json.Linq.JObject.Parse(uploadJson);
                string? filePath = (string?)jObj["filePath"] ?? (string?)jObj["FilePath"];
                string? fileName = (string?)jObj["fileName"] ?? (string?)jObj["FileName"] ?? file.FileName;
                return Json(new { success = true, filePath, fileName });
            }
            else
            {
                var errorMsg = await uploadResponse.Content.ReadAsStringAsync();
                return Json(new { success = false, message = errorMsg });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Dosya yuklenirken hata olustu: {ex.Message}" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] CreateMesajDto dto)
    {
        if (dto == null || (string.IsNullOrWhiteSpace(dto.Icerik) && string.IsNullOrWhiteSpace(dto.DosyaYolu)) || dto.StajBasvuruId <= 0)
        {
            return Json(new { success = false, message = "Mesaj icerigi veya dosya bos olamaz." });
        }

        var client = CreateApiClient(out var token);
        if (string.IsNullOrEmpty(token))
        {
            return Json(new { success = false, message = "Oturum sureniz dolmus, lutfen yeniden giris yapin." });
        }

        if (!UserPermissionHelper.CanAccessMesajlar(token))
        {
            return Json(new { success = false, message = "Mesajlasma yetkiniz bulunmamaktadir." });
        }

        int adminId = LoginController.GetUserIdFromToken(token) ?? 1;
        dto.GonderenId = adminId;

        if (dto.AliciId <= 0)
        {
            var infoRes = await client.GetAsync($"{_baseUrl}/api/Mesajlar/BasvuruBilgi/{dto.StajBasvuruId}");
            if (infoRes.IsSuccessStatusCode)
            {
                var infoJson = await infoRes.Content.ReadAsStringAsync();
                var infoObj = JsonConvert.DeserializeObject<OgrenciBasvuruBilgiDto>(infoJson);
                if (infoObj?.UserId != null && infoObj.UserId > 0)
                {
                    dto.AliciId = infoObj.UserId.Value;
                }
            }
        }

        dto.GonderenRol = "Admin";
        dto.GonderenAd = string.IsNullOrWhiteSpace(dto.GonderenAd) ? "Staj Koordinasyonu" : dto.GonderenAd;

        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{_baseUrl}/api/Mesajlar", content);
        if (response.IsSuccessStatusCode)
        {
            var resJson = await response.Content.ReadAsStringAsync();
            var created = JsonConvert.DeserializeObject<ResultMesajDto>(resJson);
            return Json(new { success = true, data = created });
        }

        return Json(new { success = false, message = "Mesaj gonderilemedi." });
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages(int stajBasvuruId, int? projeId = null)
    {
        if (stajBasvuruId <= 0 || !projeId.HasValue || projeId.Value <= 0) return Json(new List<ResultMesajDto>());

        var client = CreateApiClient(out var token);
        if (!UserPermissionHelper.CanAccessMesajlar(token))
        {
            return Json(new List<ResultMesajDto>());
        }

        int adminId = LoginController.GetUserIdFromToken(token) ?? 1;
        string url = $"{_baseUrl}/api/Mesajlar/Konusma/{stajBasvuruId}?projeId={projeId.Value}&okuyanUserId={adminId}";

        var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<ResultMesajDto>>(json) ?? new List<ResultMesajDto>();
            return Json(list);
        }

        return Json(new List<ResultMesajDto>());
    }
}
