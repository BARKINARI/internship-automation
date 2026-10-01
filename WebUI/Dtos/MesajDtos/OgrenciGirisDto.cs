using System.ComponentModel.DataAnnotations;

namespace WebUI.Dtos.MesajDtos;

public class OgrenciGirisDto
{
    [Required(ErrorMessage = "Lütfen staj başvurunuzda kullandığınız e-posta adresinizi giriniz.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Eposta { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lütfen staj başvurunuzda kullandığınız telefon numaranızı giriniz.")]
    public string Telefon { get; set; } = string.Empty;
}
