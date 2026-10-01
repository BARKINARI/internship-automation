using System.ComponentModel.DataAnnotations;

namespace WebUI.Dtos.ProjeDtos;

public class UpdateProjeDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Proje başlığı zorunludur.")]
    [MaxLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
    public string Baslik { get; set; } = string.Empty;

    [Required(ErrorMessage = "Proje açıklaması zorunludur.")]
    public string Aciklama { get; set; } = string.Empty;

    [Required(ErrorMessage = "Başlangıç tarihi zorunludur.")]
    public DateTime BaslangicTarihi { get; set; }

    [Required(ErrorMessage = "Bitiş tarihi zorunludur.")]
    public DateTime BitisTarihi { get; set; }

    public int Durum { get; set; }
    public int? YoneticiId { get; set; }
}
