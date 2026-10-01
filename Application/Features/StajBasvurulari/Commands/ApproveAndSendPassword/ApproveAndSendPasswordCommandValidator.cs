using FluentValidation;

namespace Application.Features.StajBasvurulari.Commands.ApproveAndSendPassword;

public class ApproveAndSendPasswordCommandValidator : AbstractValidator<ApproveAndSendPasswordCommand>
{
    public ApproveAndSendPasswordCommandValidator()
    {
        RuleFor(c => c.StajBasvuruId).GreaterThan(0);
        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("Şifre alanı boş bırakılamaz.")
            .MinimumLength(6).WithMessage("Öğrenciye atanacak şifre en az 6 karakter olmalıdır.");
    }
}