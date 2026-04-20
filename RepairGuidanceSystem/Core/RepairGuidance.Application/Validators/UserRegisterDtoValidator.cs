using FluentValidation;
using RepairGuidance.Application.Dtos;

namespace RepairGuidance.Application.Validators
{
    public class UserRegisterDtoValidator : AbstractValidator<UserRegisterDto>
    {
        public UserRegisterDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Ad alanı boş bırakılamaz.")
                .MaximumLength(50)
                .WithMessage("Ad 50 karakterden uzun olamaz.");

            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Soyad alanı boş bırakılamaz.")
                .MaximumLength(50)
                .WithMessage("Soyad 50 karakterden uzun olamaz.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("E-posta adresi gereklidir.")
                .EmailAddress()
                .WithMessage("Geçerli bir e-posta adresi giriniz.");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Şifre boş bırakılamaz.")
                .MinimumLength(8)
                .WithMessage("Şifre en az 8 karakter olmalıdır.")
                .Matches(@"[A-Z]+")
                .WithMessage("Şifre en az bir büyük harf içermelidir.")
                .Matches(@"[0-9]+")
                .WithMessage("Şifre en az bir rakam içermelidir.")
                .Matches(@"[\!\?\*\.]强度")
                .WithMessage("Şifre en az bir özel karakter (!?*.) içermelidir.");
        }
    }
}
