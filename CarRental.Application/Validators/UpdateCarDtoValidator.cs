using CarRental.Application.Features;
using FluentValidation;

namespace CarRental.Application.Validators;

public class UpdateCarDtoValidator : AbstractValidator<UpdateCarDto>
{
    public UpdateCarDtoValidator()
    {
        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("License plate is required.")
            .MaximumLength(20).WithMessage("License plate must be at most 20 characters.");

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required.")
            .MaximumLength(100).WithMessage("Brand must be at most 100 characters.");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required.")
            .MaximumLength(100).WithMessage("Model must be at most 100 characters.");

        RuleFor(x => x.DistanceKm)
            .GreaterThanOrEqualTo(0).WithMessage("DistanceKm cannot be negative.");

        RuleFor(x => x.DailyPrice)
            .GreaterThan(0).WithMessage("DailyPrice must be greater than 0.");

        RuleFor(x => x.Status)
            .InclusiveBetween(0, 2).WithMessage("Status must be a valid car status value.");

        RuleFor(x => x.UnavailableNote)
            .MaximumLength(500).WithMessage("Unavailable note must be at most 500 characters.");

        When(x => x.UnavailableFrom.HasValue || x.UnavailableTo.HasValue, () =>
        {
            RuleFor(x => x.UnavailableFrom)
                .NotNull().WithMessage("UnavailableFrom is required when UnavailableTo is provided.");

            RuleFor(x => x.UnavailableTo)
                .NotNull().WithMessage("UnavailableTo is required when UnavailableFrom is provided.");

            RuleFor(x => x)
                .Must(x => x.UnavailableFrom < x.UnavailableTo)
                .WithMessage("UnavailableFrom must be before UnavailableTo.");
        });

        When(x => x.UnavailableReason.HasValue, () =>
        {
            RuleFor(x => x.UnavailableReason!.Value)
                .InclusiveBetween(1, 6).WithMessage("UnavailableReason must be a valid value.");
        });
    }
}