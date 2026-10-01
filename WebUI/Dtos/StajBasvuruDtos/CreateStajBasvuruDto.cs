namespace WebUI.Dtos.StajBasvuruDtos;

public class CreateStajBasvuruDto
{
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string Eposta { get; set; } = string.Empty;
    public string Okul { get; set; } = string.Empty;
    public string Bolum { get; set; } = string.Empty;
    public string Sinif { get; set; } = string.Empty;
    public DateTime StajBaslangicTarihi { get; set; } = DateTime.Today;
    public DateTime StajBitisTarihi { get; set; } = DateTime.Today.AddDays(30);
    public string? Referans { get; set; }
    public string? KendiniTanitmaYazisi { get; set; }
    public string? CvDosyaYolu { get; set; }
    public string? StajKonusu { get; set; }
    public int StajBasvuruDurumu { get; set; } = 1;
}