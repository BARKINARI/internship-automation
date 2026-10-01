namespace Application.Features.Auth.Constants;

public static class AuthMessages
{
    public const string EmailAlreadyExists = "Bu e-posta adresi zaten kayıtlı.";
    public const string UserNotFound = "Kullanıcı bulunamadı.";
    public const string PasswordDontMatch = "Şifre hatalı.";
    public const string RefreshTokenNotFound = "Refresh token bulunamadı.";
    public const string RefreshTokenNotActive = "Refresh token aktif değil veya süresi dolmuş.";
}