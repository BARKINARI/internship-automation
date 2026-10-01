namespace Application.Features.Mesajlar.Commands.Create;

public class CreatedMesajResponse
{
    public int Id { get; set; }
    public int GonderenId { get; set; }
    public string GonderenAd { get; set; } = string.Empty;
    public int AliciId { get; set; }
    public string AliciAd { get; set; } = string.Empty;
    public int? StajBasvuruId { get; set; }
    public int? ProjeId { get; set; }
    public string Icerik { get; set; } = string.Empty;
    public string? DosyaYolu { get; set; }
    public string? DosyaAdi { get; set; }
    public bool OkunduMu { get; set; }
    public DateTime CreatedDate { get; set; }
}
