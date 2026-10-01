using FluentValidation;

namespace Application.Features.Projeler.Commands.Update;

public class UpdateProjeCommandValidator : AbstractValidator<UpdateProjeCommand>
{
    public UpdateProjeCommandValidator()
    {
        RuleFor(p => p.Id).GreaterThan(0);
        RuleFor(p => p.Baslik).NotEmpty().MaximumLength(200);
        RuleFor(p => p.Aciklama).NotEmpty().MaximumLength(4000);
        RuleFor(p => p.BaslangicTarihi).NotEmpty();
        RuleFor(p => p.BitisTarihi).NotEmpty().GreaterThanOrEqualTo(p => p.BaslangicTarihi);
    }
}
