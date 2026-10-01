using FluentValidation;

namespace Application.Features.Mesajlar.Commands.Create;

public class CreateMesajCommandValidator : AbstractValidator<CreateMesajCommand>
{
    public CreateMesajCommandValidator()
    {
        RuleFor(m => m.GonderenId).GreaterThan(0);
        RuleFor(m => m.AliciId).GreaterThan(0);
        RuleFor(m => m.Icerik)
            .NotEmpty()
            .When(m => string.IsNullOrEmpty(m.DosyaYolu))
            .WithMessage("Mesaj içeriği veya dosya eklemelisiniz.");
        RuleFor(m => m.Icerik).MaximumLength(4000);
    }
}
