using FluentValidation;

namespace Application.Features.StajBasvurulari.Commands.Update;

public class UpdateStajBasvuruCommandValidator : AbstractValidator<UpdateStajBasvuruCommand>
{
    public UpdateStajBasvuruCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty().GreaterThan(0);
        RuleFor(c => c.Ad).NotEmpty().MinimumLength(2);
        RuleFor(c => c.Soyad).NotEmpty().MinimumLength(2);
        RuleFor(c => c.Telefon).NotEmpty();
        RuleFor(c => c.Eposta).NotEmpty().EmailAddress();
    }
}