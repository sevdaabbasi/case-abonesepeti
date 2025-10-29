using FluentValidation;
using Auth.Api.Dtos.Requests;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefon boş olamaz.")
            .Matches(@"^\+?\d{10,15}$").WithMessage("Geçerli bir telefon numarası girin."); 

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(10).WithMessage("Parola en az 10 karakter olmalı.")
            .Matches("[A-Z]").WithMessage("En az bir büyük harf içermeli.")
            .Matches("[a-z]").WithMessage("En az bir küçük harf içermeli.")
            .Matches("[0-9]").WithMessage("En az bir rakam içermeli.")
            .Matches("[^a-zA-Z0-9]").WithMessage("En az bir özel karakter içermeli.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.Password).WithMessage("Parolalar eşleşmiyor.");

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(r => r == "User" || r == "Admin")
            .WithMessage("Role ya 'User' ya da 'Admin' olmalı.");
    }
}