using Domain.Enums;

namespace Application.Features.StajBasvurulari.Queries.GetList;

public class GetListStajBasvuruListItemDto
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string Eposta { get; set; } = string.Empty;
    public string Okul { get; set; } = string.Empty;
    public string Bolum { get; set; } = string.Empty;
    public string Sinif { get; set; } = string.Empty;
    public DateTime StajBaslangicTarihi { get; set; }
    public DateTime StajBitisTarihi { get; set; }
    public string? Referans { get; set; }
    public string? KendiniTanitmaYazisi { get; set; }
    public string? CvDosyaYolu { get; set; }
    public string? StajKonusu { get; set; }
    public StajBasvuruDurumu StajBasvuruDurumu { get; set; }
}