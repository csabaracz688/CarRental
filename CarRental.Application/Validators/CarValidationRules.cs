using CarRental.Application.Features;
using FluentValidation;

namespace CarRental.Application.Validators;

public static class CarValidationRules
{
    public static void AddCreateCarRules(this AbstractValidator<CreateCarDto> validator)
    {
        validator.RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("License plate is required.")
            .MaximumLength(20).WithMessage("License plate must be at most 20 characters.");

        validator.RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required.")
            .MaximumLength(100).WithMessage("Brand must be at most 100 characters.");

        validator.RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required.")
            .MaximumLength(100).WithMessage("Model must be at most 100 characters.");

        validator.RuleFor(x => x.DistanceKm)
            .GreaterThanOrEqualTo(0).WithMessage("DistanceKm cannot be negative.");

        validator.RuleFor(x => x.DailyPrice)
            .GreaterThan(0).WithMessage("DailyPrice must be greater than 0.");

        validator.RuleFor(x => x.Status)
            .InclusiveBetween(0, 2).WithMessage("Status must be a valid car status value.");

        validator.RuleFor(x => x.UnavailableNote)
            .MaximumLength(500).WithMessage("Unavailable note must be at most 500 characters.");

        validator.When(x => x.UnavailableFrom.HasValue || x.UnavailableTo.HasValue, () =>
        {
            validator.RuleFor(x => x.UnavailableFrom)
                .NotNull().WithMessage("UnavailableFrom is required when UnavailableTo is provided.");

            validator.RuleFor(x => x.UnavailableTo)
                .NotNull().WithMessage("UnavailableTo is required when UnavailableFrom is provided.");

            validator.RuleFor(x => x)
                .Must(x => x.UnavailableFrom < x.UnavailableTo)
                .WithMessage("UnavailableFrom must be before UnavailableTo.");
        });

        validator.When(x => x.UnavailableReason.HasValue, () =>
        {
            validator.RuleFor(x => x.UnavailableReason!.Value)
                .InclusiveBetween(1, 6).WithMessage("UnavailableReason must be a valid value.");
        });
    }

    public static void AddUpdateCarRules(this AbstractValidator<UpdateCarDto> validator)
    {
        validator.RuleFor(x => x.Id)
            .GreaterThan(0)
            .When(x => x.Id.HasValue)
            .WithMessage("Id must be greater than 0.");

        validator.RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("License plate is required.")
            .MaximumLength(20).WithMessage("License plate must be at most 20 characters.");

        validator.RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required.")
            .MaximumLength(100).WithMessage("Brand must be at most 100 characters.");

        validator.RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required.")
            .MaximumLength(100).WithMessage("Model must be at most 100 characters.");

        validator.RuleFor(x => x.DistanceKm)
            .GreaterThanOrEqualTo(0).WithMessage("DistanceKm cannot be negative.");

        validator.RuleFor(x => x.DailyPrice)
            .GreaterThan(0).WithMessage("DailyPrice must be greater than 0.");

        validator.RuleFor(x => x.Status)
            .InclusiveBetween(0, 2).WithMessage("Status must be a valid car status value.");

        validator.RuleFor(x => x.UnavailableNote)
            .MaximumLength(500).WithMessage("Unavailable note must be at most 500 characters.");

        validator.When(x => x.UnavailableFrom.HasValue || x.UnavailableTo.HasValue, () =>
        {
            validator.RuleFor(x => x.UnavailableFrom)
                .NotNull().WithMessage("UnavailableFrom is required when UnavailableTo is provided.");

            validator.RuleFor(x => x.UnavailableTo)
                .NotNull().WithMessage("UnavailableTo is required when UnavailableFrom is provided.");

            validator.RuleFor(x => x)
                .Must(x => x.UnavailableFrom < x.UnavailableTo)
                .WithMessage("UnavailableFrom must be before UnavailableTo.");
        });

        validator.When(x => x.UnavailableReason.HasValue, () =>
        {
            validator.RuleFor(x => x.UnavailableReason!.Value)
                .InclusiveBetween(1, 6).WithMessage("UnavailableReason must be a valid value.");
        });
    }
}