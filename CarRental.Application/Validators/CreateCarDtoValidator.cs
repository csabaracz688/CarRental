using CarRental.Application.Features;
using FluentValidation;

namespace CarRental.Application.Validators;

public class CreateCarDtoValidator : AbstractValidator<CreateCarDto>
{
    public CreateCarDtoValidator()
    {
        this.AddCreateCarRules();
    }
}