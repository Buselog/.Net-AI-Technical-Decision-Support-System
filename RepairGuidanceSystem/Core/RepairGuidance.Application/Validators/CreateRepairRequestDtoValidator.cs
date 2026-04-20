using FluentValidation;
using RepairGuidance.Application.Dtos;

namespace RepairGuidance.Application.Validators
{
    public class CreateRepairRequestDtoValidator : AbstractValidator<CreateRepairRequestDto>
    {
        public CreateRepairRequestDtoValidator()
        {
            RuleFor(x => x.DeviceName)
               .Cascade(CascadeMode.Stop)
               .NotEmpty()
               .WithMessage("Cihaz adı belirtilmelidir.")
               .MinimumLength(3)
               .WithMessage("Cihaz adı çok kısa.");

            RuleFor(x => x.ProblemDescription)
               .Cascade(CascadeMode.Stop)
               .NotEmpty()
               .WithMessage("Arıza tanımı boş olamaz.")
               .MinimumLength(10)
               .WithMessage("Lütfen arızayı biraz daha detaylı açıklayın (En az 10 karakter).");

            RuleFor(x => x.TargetLevel)
               .NotEmpty()
               .WithMessage("Lütfen bir zorluk seviyesi seçin (Acemi, Orta, Uzman).");
        }
    }
}
