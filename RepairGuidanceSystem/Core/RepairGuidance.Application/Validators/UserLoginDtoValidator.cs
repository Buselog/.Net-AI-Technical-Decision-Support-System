using FluentValidation;
using RepairGuidance.Application.Dtos;

namespace RepairGuidance.Application.Validators
{
    public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
    {
        public UserLoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("E-posta alanı boş bırakılamaz.")
                .EmailAddress()
                .WithMessage("Geçerli bir e-posta adresi giriniz.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Şifre alanı boş bırakılamaz.");
        }
    }
}
