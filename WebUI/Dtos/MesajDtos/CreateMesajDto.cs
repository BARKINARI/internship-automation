namespace WebUI.Dtos.MesajDtos;

public class CreateMesajDto
{
    public int GonderenId { get; set; }
    public int AliciId { get; set; }
    public int? StajBasvuruId { get; set; }
    public int? ProjeId { get; set; }
    public string Icerik { get; set; } = string.Empty;
    public string? DosyaYolu { get; set; }
    public string? DosyaAdi { get; set; }

    public string? GonderenRol { get; set; }
    public string? GonderenAd { get; set; }
}
