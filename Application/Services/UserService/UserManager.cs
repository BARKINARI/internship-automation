using Application.Services.Repositories;
using Core.Security.Entities;

namespace Application.Services.UserService;

public class UserManager : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserManager(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User<int>?> GetByEmail(string email)
    {
        User<int>? user = await _userRepository.GetAsync(u => u.Email == email);
        return user;
    }

    public async Task<User<int>?> GetById(int id)
    {
        User<int>? user = await _userRepository.GetAsync(u => u.Id == id);
        return user;
    }

    public async Task<User<int>> Add(User<int> user)
    {
        User<int> addedUser = await _userRepository.AddAsync(user);
        return addedUser;
    }

    public async Task<User<int>> Update(User<int> user)
    {
        User<int> updatedUser = await _userRepository.UpdateAsync(user);
        return updatedUser;
    }
}