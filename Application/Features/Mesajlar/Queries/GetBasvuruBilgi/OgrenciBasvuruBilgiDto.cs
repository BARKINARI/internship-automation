namespace Application.Features.Mesajlar.Queries.GetBasvuruBilgi;

public class OgrenciBasvuruBilgiDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string AdSoyad { get; set; } = string.Empty;
    public string Eposta { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string Okul { get; set; } = string.Empty;
    public string Bolum { get; set; } = string.Empty;
    public string Sinif { get; set; } = string.Empty;
    public string? StajKonusu { get; set; }
    public DateTime StajBaslangicTarihi { get; set; }
    public DateTime StajBitisTarihi { get; set; }
    public int StajBasvuruDurumu { get; set; }
}
