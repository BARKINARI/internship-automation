namespace WebUI.Dtos.MesajDtos;

public class OgrenciGirisSonucDto
{
    public bool Basarili { get; set; }
    public string Mesaj { get; set; } = string.Empty;
    public int? StajBasvuruId { get; set; }
    public string? OgrenciAdSoyad { get; set; }
    public string? Okul { get; set; }
    public string? Bolum { get; set; }
    public string? StajKonusu { get; set; }
    public int? StajBasvuruDurumu { get; set; }
}
