namespace WebAPI;

public class WebAPIConfiguration
{
    public string APIDomain { get; set; } = string.Empty;
    public string[] AllowedOrigins { get; set; } = [];
}