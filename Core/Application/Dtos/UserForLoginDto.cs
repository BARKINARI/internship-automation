namespace Core.Application.Dtos;

public class UserForLoginDto : IDto
{
    public string Email { get; set; }
    public string Password { get; set; }

    public UserForLoginDto()
    {
    }

    public UserForLoginDto(string email, string password)
    {
        Email = email;
        Password = password;
    }
}