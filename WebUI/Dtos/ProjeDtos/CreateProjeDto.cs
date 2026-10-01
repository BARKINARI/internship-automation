using System.ComponentModel.DataAnnotations;

namespace WebUI.Dtos.ProjeDtos;

public class CreateProjeDto
{
    [Required(ErrorMessage = "Proje başlığı zorunludur.")]
    [MaxLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
    public string Baslik { get; set; } = string.Empty;

    [Required(ErrorMessage = "Proje açıklaması zorunludur.")]
    public string Aciklama { get; set; } = string.Empty;

    [Required(ErrorMessage = "Başlangıç tarihi zorunludur.")]
    public DateTime BaslangicTarihi { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Bitiş tarihi zorunludur.")]
    public DateTime BitisTarihi { get; set; } = DateTime.Today.AddDays(14);

    public int Durum { get; set; } = 1;
    public int? YoneticiId { get; set; }
}
