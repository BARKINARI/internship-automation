using System.ComponentModel.DataAnnotations;

namespace WebUI.Dtos.UserDtos;

public class SetUserRolesDto
{
    [Required]
    public int UserId { get; set; }

    public List<int> OperationClaimIds { get; set; } = new();
}

public class CreateUserWithRolesDto
{
    [Required(ErrorMessage = "Ad zorunludur.")]
    [MaxLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [MaxLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    public string Password { get; set; } = "Staj2026*";

    public List<int> OperationClaimIds { get; set; } = new();
}
