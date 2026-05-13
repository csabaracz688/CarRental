using CarRental.Application.Features;
using FluentValidation;

namespace CarRental.Application.Validators;

public class UpdateCarDtoValidator : AbstractValidator<UpdateCarDto>
{
    public UpdateCarDtoValidator()
    {
        this.AddUpdateCarRules();
    }
}