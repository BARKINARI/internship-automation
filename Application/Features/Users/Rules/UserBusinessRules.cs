using Application.Features.Users.Constants;
using Application.Services.Repositories;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exception.Types;
using Core.Security.Entities;
using Core.Security.Hashing;

namespace Application.Features.Users.Rules;

public class UserBusinessRules : BaseBusinessRules
{
    private readonly IUserRepository _userRepository;

    public UserBusinessRules(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public void UserShouldExistWhenSelected(User<int>? user)
    {
        if (user == null)
            throw new BusinessException(UsersMessages.UserDontExists);
    }

    public async Task UserIdShouldExistWhenSelected(int id)
    {
        User<int>? user = await _userRepository.GetAsync(predicate: u => u.Id == id, enableTracking: false);
        UserShouldExistWhenSelected(user);
    }

    public async Task UserEmailShouldNotExistsWhenInsert(string email)
    {
        bool exists = await _userRepository.AnyAsync(predicate: u => u.Email == email);
        if (exists)
            throw new BusinessException(UsersMessages.UserMailAlreadyExists);
    }

    public async Task UserEmailShouldNotExistsWhenUpdate(int id, string email)
    {
        bool exists = await _userRepository.AnyAsync(predicate: u => u.Id != id && u.Email == email);
        if (exists)
            throw new BusinessException(UsersMessages.UserMailAlreadyExists);
    }

    public void UserPasswordShouldMatchToOldPassword(User<int> user, string password)
    {
        if (!HashingHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            throw new BusinessException(UsersMessages.PasswordDontMatch);
    }
}