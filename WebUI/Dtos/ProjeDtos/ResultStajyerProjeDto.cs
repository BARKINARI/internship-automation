namespace WebUI.Dtos.ProjeDtos;

public class ResultStajyerProjeDto
{
    public int StajyerProjeId { get; set; }
    public int ProjeId { get; set; }
    public string ProjeBaslik { get; set; } = string.Empty;
    public string ProjeAciklama { get; set; } = string.Empty;
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public int StajBasvuruId { get; set; }
    public string StajyerAdSoyad { get; set; } = string.Empty;
    public string StajyerEposta { get; set; } = string.Empty;
    public DateTime AtanmaTarihi { get; set; }
    public DateTime? TeslimTarihi { get; set; }
    public int Durum { get; set; }
    public string? TeslimAciklamasi { get; set; }
    public string? DosyaYolu { get; set; }
    public int? Puan { get; set; }
    public string? YoneticiNotu { get; set; }

    public string DurumMetni => Durum switch
    {
        1 => "Atandı",
        2 => "Devam Ediyor",
        3 => "Teslim Edildi",
        4 => "Onaylandı",
        5 => "Revize İsteniyor",
        _ => "Belirsiz"
    };

    public string DurumRozet => Durum switch
    {
        1 => "badge-info",
        2 => "badge-primary",
        3 => "badge-warning",
        4 => "badge-success",
        5 => "badge-danger",
        _ => "badge-secondary"
    };
}
