using FluentValidation;

namespace Application.Features.StajBasvurulari.Commands.Create;

public class CreateStajBasvuruCommandValidator : AbstractValidator<CreateStajBasvuruCommand>
{
    public CreateStajBasvuruCommandValidator()
    {
        RuleFor(c => c.Ad).NotEmpty().MinimumLength(2).MaximumLength(50);
        RuleFor(c => c.Soyad).NotEmpty().MinimumLength(2).MaximumLength(50);
        RuleFor(c => c.Telefon).NotEmpty().MinimumLength(10).MaximumLength(20);
        RuleFor(c => c.Eposta).NotEmpty().EmailAddress();
        RuleFor(c => c.Okul).NotEmpty().MinimumLength(2);
        RuleFor(c => c.Bolum).NotEmpty().MinimumLength(2);
        RuleFor(c => c.Sinif).NotEmpty();
        RuleFor(c => c.StajBaslangicTarihi).NotEmpty();
        RuleFor(c => c.StajBitisTarihi).NotEmpty();
    }
}