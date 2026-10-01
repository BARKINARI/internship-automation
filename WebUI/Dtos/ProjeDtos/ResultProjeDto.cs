namespace WebUI.Dtos.ProjeDtos;

public class ResultProjeDto
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public int Durum { get; set; }
    public int? YoneticiId { get; set; }
    public string? YoneticiAd { get; set; }
    public int AtananStajyerSayisi { get; set; }
    public DateTime CreatedDate { get; set; }

    public string DurumMetni => Durum switch
    {
        1 => "Beklemede",
        2 => "Devam Ediyor",
        3 => "Tamamlandı",
        4 => "İptal Edildi",
        _ => "Belirsiz"
    };

    public string DurumRozet => Durum switch
    {
        1 => "badge-warning",
        2 => "badge-primary",
        3 => "badge-success",
        4 => "badge-secondary",
        _ => "badge-light"
    };
}
