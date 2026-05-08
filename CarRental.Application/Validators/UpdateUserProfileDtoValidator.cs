using CarRental.Application.Users;
using FluentValidation;

namespace CarRental.Application.Validators;

public class UpdateUserProfileDtoValidator : AbstractValidator<UpdateUserProfileDto>
{
    public UpdateUserProfileDtoValidator()
    {
        RuleFor(x => x.PostalCode)
            .GreaterThan(0)
            .When(x => x.PostalCode.HasValue)
            .WithMessage("Postal code must be greater than 0.");

        RuleFor(x => x.City)
            .MaximumLength(100)
            .WithMessage("City must be at most 100 characters.");

        RuleFor(x => x.Address)
            .MaximumLength(200)
            .WithMessage("Address must be at most 200 characters.");

        RuleFor(x => x.Phone)
            .MaximumLength(30)
            .WithMessage("Phone must be at most 30 characters.");
    }
}