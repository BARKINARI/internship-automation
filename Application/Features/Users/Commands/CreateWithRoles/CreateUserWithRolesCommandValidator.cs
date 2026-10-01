using FluentValidation;

namespace Application.Features.Users.Commands.CreateWithRoles;

public class CreateUserWithRolesCommandValidator : AbstractValidator<CreateUserWithRolesCommand>
{
    public CreateUserWithRolesCommandValidator()
    {
        RuleFor(c => c.FirstName).NotEmpty().MinimumLength(2);
        RuleFor(c => c.LastName).NotEmpty().MinimumLength(2);
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("Şifre alanı boş bırakılamaz.")
            .MinimumLength(6).WithMessage("Kullanıcı şifresi en az 6 karakter olmalıdır.");
    }
}