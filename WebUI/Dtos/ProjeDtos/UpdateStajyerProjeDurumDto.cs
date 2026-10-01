namespace WebUI.Dtos.ProjeDtos;

public class UpdateStajyerProjeDurumDto
{
    public int StajyerProjeId { get; set; }
    public int Durum { get; set; }
    public string? TeslimAciklamasi { get; set; }
    public string? DosyaYolu { get; set; }
    public int? Puan { get; set; }
    public string? YoneticiNotu { get; set; }
}
