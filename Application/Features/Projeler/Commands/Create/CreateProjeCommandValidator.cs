using FluentValidation;

namespace Application.Features.Projeler.Commands.Create;

public class CreateProjeCommandValidator : AbstractValidator<CreateProjeCommand>
{
    public CreateProjeCommandValidator()
    {
        RuleFor(p => p.Baslik).NotEmpty().MaximumLength(200);
        RuleFor(p => p.Aciklama).NotEmpty().MaximumLength(4000);
        RuleFor(p => p.BaslangicTarihi).NotEmpty();
        RuleFor(p => p.BitisTarihi).NotEmpty().GreaterThanOrEqualTo(p => p.BaslangicTarihi);
    }
}
