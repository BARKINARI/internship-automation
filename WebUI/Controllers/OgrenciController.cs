using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WebUI.Dtos.MesajDtos;
using WebUI.Dtos.ProjeDtos;

namespace WebUI.Controllers;

public class OgrenciController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseUrl;

    public OgrenciController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _baseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://localhost:7125";
    }

    [HttpGet]
    public IActionResult Giris()
    {
        if (Request.Cookies.ContainsKey("ogrenci_basvuru_id"))
        {
            return RedirectToAction("Mesajlar");
        }
        return RedirectToAction("Index", "Login");
    }

    [HttpPost]
    public IActionResult Giris(OgrenciGirisDto dto)
    {
        return RedirectToAction("Index", "Login");
    }

    [HttpGet]
    public async Task<IActionResult> Mesajlar(int? projeId = null)
    {
        if (!Request.Cookies.TryGetValue("ogrenci_basvuru_id", out var basvuruIdStr) ||
            !int.TryParse(basvuruIdStr, out var basvuruId) || basvuruId <= 0)
        {
            return RedirectToAction("Giris");
        }

        var client = _httpClientFactory.CreateClient();

        int? userId = null;
        if (Request.Cookies.TryGetValue("ogrenci_user_id", out var userIdStr) && int.TryParse(userIdStr, out var uidVal))
        {
            userId = uidVal;
        }

        var infoRes = await client.GetAsync($"{_baseUrl}/api/Mesajlar/BasvuruBilgi/{basvuruId}");
        if (infoRes.IsSuccessStatusCode)
        {
            var infoJson = await infoRes.Content.ReadAsStringAsync();
            var infoObj = JsonConvert.DeserializeObject<OgrenciBasvuruBilgiDto>(infoJson);
            ViewBag.OgrenciBilgi = infoObj;
            if (!userId.HasValue && infoObj?.UserId != null)
            {
                userId = infoObj.UserId;
            }
        }

        ViewBag.UserId = userId;

        List<ResultStajyerProjeDto> projeler = new();
        var projeRes = await client.GetAsync($"{_baseUrl}/api/Projeler/StajyerProjeleri?stajBasvuruId={basvuruId}");
        if (projeRes.IsSuccessStatusCode)
        {
            var projeJson = await projeRes.Content.ReadAsStringAsync();
            projeler = JsonConvert.DeserializeObject<List<ResultStajyerProjeDto>>(projeJson) ?? new List<ResultStajyerProjeDto>();
        }

        ViewBag.Projeler = projeler;
        ViewBag.BasvuruId = basvuruId;
        ViewBag.OgrenciAdSoyad = Request.Cookies["ogrenci_ad_soyad"] ?? "Öğrenci";

        List<ResultMesajDto> allMesajlar = new();
        var msgRes = await client.GetAsync($"{_baseUrl}/api/Mesajlar/Konusma/{basvuruId}");
        if (msgRes.IsSuccessStatusCode)
        {
            var msgJson = await msgRes.Content.ReadAsStringAsync();
            allMesajlar = JsonConvert.DeserializeObject<List<ResultMesajDto>>(msgJson) ?? new List<ResultMesajDto>();
        }

        Dictionary<int, int> unreadCounts = new();
        Dictionary<int, string> lastMsgTimes = new();

        foreach (var p in projeler)
        {
            var pMsgs = allMesajlar.Where(m => m.ProjeId == p.ProjeId).ToList();
            int unread = userId.HasValue
                ? pMsgs.Count(m => !m.OkunduMu && m.AliciId == userId.Value)
                : pMsgs.Count(m => !m.OkunduMu && m.GonderenRol != "Ogrenci");

            unreadCounts[p.ProjeId] = unread;
            var last = pMsgs.LastOrDefault();
            if (last != null)
            {
                lastMsgTimes[p.ProjeId] = last.CreatedDate.ToString("HH:mm");
            }
        }

        ViewBag.UnreadCounts = unreadCounts;
        ViewBag.LastMsgTimes = lastMsgTimes;
        ViewBag.TotalUnread = unreadCounts.Values.Sum();
        ViewBag.InitialProjeId = projeId;

        return View(new List<ResultMesajDto>());
    }

    [HttpPost]
    public async Task<IActionResult> UploadMesajDosya(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Json(new { success = false, message = "Lütfen geçerli bir dosya seçiniz." });

        if (!Request.Cookies.TryGetValue("ogrenci_basvuru_id", out var basvuruIdStr) ||
            !int.TryParse(basvuruIdStr, out var basvuruId) || basvuruId <= 0)
        {
            return Json(new { success = false, message = "Oturumunuz sonlanmış, lütfen tekrar giriş yapın." });
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
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
            return Json(new { success = false, message = $"Dosya yüklenirken hata oluştu: {ex.Message}" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] CreateMesajDto dto)
    {
        if (!Request.Cookies.TryGetValue("ogrenci_basvuru_id", out var basvuruIdStr) ||
            !int.TryParse(basvuruIdStr, out var basvuruId) || basvuruId <= 0)
        {
            return Json(new { success = false, message = "Oturumunuz sonlanmış, lütfen tekrar giriş yapın." });
        }

        if (dto == null || (string.IsNullOrWhiteSpace(dto.Icerik) && string.IsNullOrWhiteSpace(dto.DosyaYolu)) || !dto.ProjeId.HasValue || dto.ProjeId.Value <= 0)
        {
            return Json(new { success = false, message = "Lütfen mesaj göndermek için bir proje seçiniz ve mesaj veya dosya ekleyiniz." });
        }

        var client = _httpClientFactory.CreateClient();

        int senderUserId = 0;
        if (Request.Cookies.TryGetValue("ogrenci_user_id", out var uidStr) && int.TryParse(uidStr, out var parsedUid))
        {
            senderUserId = parsedUid;
        }

        if (senderUserId <= 0)
        {
            var infoRes = await client.GetAsync($"{_baseUrl}/api/Mesajlar/BasvuruBilgi/{basvuruId}");
            if (infoRes.IsSuccessStatusCode)
            {
                var infoJson = await infoRes.Content.ReadAsStringAsync();
                var infoObj = JsonConvert.DeserializeObject<OgrenciBasvuruBilgiDto>(infoJson);
                if (infoObj?.UserId != null)
                {
                    senderUserId = infoObj.UserId.Value;
                }
            }
        }

        dto.StajBasvuruId = basvuruId;
        dto.GonderenId = senderUserId > 0 ? senderUserId : 2; 
        dto.AliciId = 1; 
        dto.GonderenRol = "Ogrenci";
        dto.GonderenAd = Request.Cookies["ogrenci_ad_soyad"] ?? "Öğrenci";

        if (dto.ProjeId.HasValue && dto.ProjeId.Value <= 0)
        {
            dto.ProjeId = null;
        }

        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{_baseUrl}/api/Mesajlar", content);
        if (response.IsSuccessStatusCode)
        {
            var resJson = await response.Content.ReadAsStringAsync();
            var created = JsonConvert.DeserializeObject<ResultMesajDto>(resJson);
            return Json(new { success = true, data = created });
        }

        return Json(new { success = false, message = "Mesaj iletilemedi." });
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages(int? projeId = null)
    {
        if (!Request.Cookies.TryGetValue("ogrenci_basvuru_id", out var basvuruIdStr) ||
            !int.TryParse(basvuruIdStr, out var basvuruId) || basvuruId <= 0 ||
            !projeId.HasValue || projeId.Value <= 0)
        {
            return Json(new List<ResultMesajDto>());
        }

        int? userId = null;
        if (Request.Cookies.TryGetValue("ogrenci_user_id", out var uidStr) && int.TryParse(uidStr, out var parsedUid))
        {
            userId = parsedUid;
        }

        var client = _httpClientFactory.CreateClient();

        if (!userId.HasValue)
        {
            var infoRes = await client.GetAsync($"{_baseUrl}/api/Mesajlar/BasvuruBilgi/{basvuruId}");
            if (infoRes.IsSuccessStatusCode)
            {
                var infoJson = await infoRes.Content.ReadAsStringAsync();
                var infoObj = JsonConvert.DeserializeObject<OgrenciBasvuruBilgiDto>(infoJson);
                if (infoObj?.UserId != null)
                {
                    userId = infoObj.UserId;
                    Response.Cookies.Append("ogrenci_user_id", userId.Value.ToString());
                }
            }
        }

        string url = userId.HasValue
            ? $"{_baseUrl}/api/Mesajlar/Konusma/{basvuruId}?projeId={projeId.Value}&okuyanUserId={userId.Value}"
            : $"{_baseUrl}/api/Mesajlar/Konusma/{basvuruId}?projeId={projeId.Value}&okuyanRol=Ogrenci";

        var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<ResultMesajDto>>(json) ?? new List<ResultMesajDto>();
            return Json(list);
        }

        return Json(new List<ResultMesajDto>());
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        if (!Request.Cookies.TryGetValue("ogrenci_basvuru_id", out var basvuruIdStr) ||
            !int.TryParse(basvuruIdStr, out var basvuruId) || basvuruId <= 0)
        {
            return Json(new List<ResultStajyerProjeDto>());
        }

        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"{_baseUrl}/api/Projeler/StajyerProjeleri?stajBasvuruId={basvuruId}");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<ResultStajyerProjeDto>>(json) ?? new List<ResultStajyerProjeDto>();
            return Json(list);
        }

        return Json(new List<ResultStajyerProjeDto>());
    }

    [HttpPost]
    public async Task<IActionResult> SubmitProject([FromBody] UpdateStajyerProjeDurumDto dto)
    {
        if (!Request.Cookies.TryGetValue("ogrenci_basvuru_id", out var basvuruIdStr) ||
            !int.TryParse(basvuruIdStr, out var basvuruId) || basvuruId <= 0)
        {
            return Json(new { success = false, message = "Oturum süreniz dolmuş, lütfen tekrar giriş yapın." });
        }

        if (dto == null || dto.StajyerProjeId <= 0)
        {
            return Json(new { success = false, message = "Geçersiz proje teslimi." });
        }

        dto.Durum = 3; 

        var client = _httpClientFactory.CreateClient();
        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PutAsync($"{_baseUrl}/api/Projeler/StajyerProjeDurum", content);
        if (response.IsSuccessStatusCode)
        {
            int senderUserId = 0;
            if (Request.Cookies.TryGetValue("ogrenci_user_id", out var uidStr) && int.TryParse(uidStr, out var parsedUid))
                senderUserId = parsedUid;

            int? projeId = null;
            var prjListRes = await client.GetAsync($"{_baseUrl}/api/Projeler/StajyerProjeleri?stajBasvuruId={basvuruId}");
            if (prjListRes.IsSuccessStatusCode)
            {
                var prjJson = await prjListRes.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<ResultStajyerProjeDto>>(prjJson);
                var found = list?.FirstOrDefault(x => x.StajyerProjeId == dto.StajyerProjeId);
                if (found != null) projeId = found.ProjeId;
            }

            var msgDto = new CreateMesajDto
            {
                StajBasvuruId = basvuruId,
                ProjeId = projeId,
                GonderenId = senderUserId > 0 ? senderUserId : 2,
                AliciId = 1,
                GonderenRol = "Ogrenci",
                GonderenAd = Request.Cookies["ogrenci_ad_soyad"] ?? "Öğrenci",
                Icerik = $"📌 Proje Görevi Teslim Edildi!\nNot: {dto.TeslimAciklamasi ?? "Teslim notu belirtilmedi."}" +
                         (!string.IsNullOrWhiteSpace(dto.DosyaYolu) ? $"\nBağlantı/Dosya: {dto.DosyaYolu}" : "")
            };

            var msgJson = JsonConvert.SerializeObject(msgDto);
            var msgContent = new StringContent(msgJson, Encoding.UTF8, "application/json");
            await client.PostAsync($"{_baseUrl}/api/Mesajlar", msgContent);

            return Json(new { success = true, message = "Projeniz başarıyla teslim edildi." });
        }

        return Json(new { success = false, message = "Proje teslim edilirken bir hata oluştu." });
    }

    [HttpGet]
    public IActionResult Cikis()
    {
        Response.Cookies.Delete("ogrenci_basvuru_id", new CookieOptions { Path = "/" });
        Response.Cookies.Delete("ogrenci_ad_soyad", new CookieOptions { Path = "/" });
        Response.Cookies.Delete("ogrenci_konu", new CookieOptions { Path = "/" });
        Response.Cookies.Delete("ogrenci_user_id", new CookieOptions { Path = "/" });
        Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
        return RedirectToAction("Index", "Login");
    }
}
