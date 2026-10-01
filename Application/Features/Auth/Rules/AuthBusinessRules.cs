using Application.Features.Auth.Constants;
using Application.Services.Repositories;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exception.Types;
using Core.Security.Entities;
using Core.Security.Hashing;

namespace Application.Features.Auth.Rules;

public class AuthBusinessRules : BaseBusinessRules
{
    private readonly IUserRepository _userRepository;

    public AuthBusinessRules(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task EmailShouldNotExistsWhenRegister(string email)
    {
        bool exists = await _userRepository.AnyAsync(u => u.Email == email);
        if (exists)
            throw new BusinessException(AuthMessages.EmailAlreadyExists);
    }

    public void UserShouldExistWhenSelected(User<int>? user)
    {
        if (user == null)
            throw new BusinessException(AuthMessages.UserNotFound);
    }

    public void UserPasswordShouldMatch(User<int> user, string password)
    {
        if (!HashingHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            throw new BusinessException(AuthMessages.PasswordDontMatch);
    }

    public void RefreshTokenShouldExist(RefreshToken<int, int>? refreshToken)
    {
        if (refreshToken == null)
            throw new BusinessException(AuthMessages.RefreshTokenNotFound);
    }

    public void RefreshTokenShouldBeActive(RefreshToken<int, int> refreshToken)
    {
        if (refreshToken.RevokedDate != null || refreshToken.ExpirationDate < DateTime.UtcNow)
            throw new BusinessException(AuthMessages.RefreshTokenNotActive);
    }
}