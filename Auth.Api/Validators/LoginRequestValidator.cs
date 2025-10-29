using FluentValidation;
using Auth.Api.Dtos.Requests;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefon gerekli.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Parola gerekli.");
    }
}