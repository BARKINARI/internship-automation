using Newtonsoft.Json;

namespace WebUI.Dtos.AuthDtos;

public class TokenDto
{
    public string Token { get; set; } = string.Empty;

    [JsonProperty("expirationDate")]
    public DateTime ExpirationDate { get; set; }

    public DateTime Expiration
    {
        get => ExpirationDate;
        set => ExpirationDate = value;
    }
}