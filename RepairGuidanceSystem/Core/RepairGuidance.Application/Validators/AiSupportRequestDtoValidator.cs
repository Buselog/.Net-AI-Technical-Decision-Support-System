using FluentValidation;
using RepairGuidance.Application.Dtos;

namespace RepairGuidance.Application.Validators
{
    public class AiSupportRequestDtoValidator : AbstractValidator<AiSupportRequestDto>
    {
        public AiSupportRequestDtoValidator()
        {
            RuleFor(x => x.UserQuestion)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Bu alan boş bırakılamaz.")
                .MinimumLength(10)
                .WithMessage("Lütfen sorunu biraz daha detaylı açıklayın (En az 10 karakter).");
        }
    }
}
