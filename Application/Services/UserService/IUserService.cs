using Core.Security.Entities;

namespace Application.Services.UserService;

public interface IUserService
{
    Task<User<int>?> GetByEmail(string email);
    Task<User<int>?> GetById(int id);
    Task<User<int>> Add(User<int> user);
    Task<User<int>> Update(User<int> user);
}