using Core.Persistence.Repositories;

namespace Core.Security.Entities;

public class User<TId> : Entity<TId>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public bool Status { get; set; } = true;

    public User()
    {
    }

    public User(string firstName, string lastName, string email, byte[] passwordSalt, byte[] passwordHash, bool status = true)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordSalt = passwordSalt;
        PasswordHash = passwordHash;
        Status = status;
    }

    public User(TId id, string firstName, string lastName, string email, byte[] passwordSalt, byte[] passwordHash, bool status = true)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordSalt = passwordSalt;
        PasswordHash = passwordHash;
        Status = status;
    }
}