using WebUI.Dtos.ProjeDtos;
using WebUI.Dtos.StajBasvuruDtos;

namespace WebUI.Dtos.MesajDtos;

public class StudentProjectChatItem
{
    public int StajBasvuruId { get; set; }
    public string OgrenciAdSoyad { get; set; } = string.Empty;
    public string Okul { get; set; } = string.Empty;
    public string? StajKonusu { get; set; }
    public int? UserId { get; set; }
    public string? Eposta { get; set; }
    public string? Telefon { get; set; }

    public int? ProjeId { get; set; }
    public string? ProjeBaslik { get; set; }
    public int? StajyerProjeDurumu { get; set; }
    public string? DurumMetni { get; set; }
    public string? DurumRozet { get; set; }

    public int UnreadCount { get; set; }
}

public class AdminMesajViewModel
{
    public List<ResultStajBasvuruDto> KabulEdilenOgrenciler { get; set; } = new();
    public List<ResultStajyerProjeDto> TumStajyerProjeleri { get; set; } = new();
    public List<StudentProjectChatItem> ProjeSohbetListesi { get; set; } = new();

    public ResultStajBasvuruDto? SeciliOgrenci { get; set; }
    public ResultStajyerProjeDto? SeciliProje { get; set; }
    public int? SeciliProjeId { get; set; }
    public List<ResultStajyerProjeDto> SeciliOgrencininProjeleri { get; set; } = new();

    public List<ResultMesajDto> Mesajlar { get; set; } = new();
    public Dictionary<int, int> OkunmamisMesajSayilari { get; set; } = new();
}
