using FluentValidation;
using Auth.Api.Dtos.Requests;

public class RefreshRequestValidator : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token gereklidir.");
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token gereklidir.");
    }
}