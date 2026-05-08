using CarRental.Application.Features;
using FluentValidation;

namespace CarRental.Application.Validators;

public class RequestRentalDtoValidator : AbstractValidator<RequestRentalDto>
{
    public RequestRentalDtoValidator()
    {
        RuleFor(x => x.CarId)
            .GreaterThan(0).WithMessage("CarId must be greater than 0.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("StartDate is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("EndDate is required.")
            .GreaterThan(x => x.StartDate).WithMessage("EndDate must be after StartDate.");

        When(x => x.UserId.HasValue, () =>
        {
            RuleFor(x => x.UserId!.Value)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");
        });

        When(x => !x.UserId.HasValue, () =>
        {
            RuleFor(x => x.GuestName)
                .NotEmpty().WithMessage("GuestName is required for guest rentals.")
                .MaximumLength(100).WithMessage("GuestName must be at most 100 characters.");

            RuleFor(x => x.GuestEmail)
                .NotEmpty().WithMessage("GuestEmail is required for guest rentals.")
                .EmailAddress().WithMessage("GuestEmail must be a valid email address.")
                .MaximumLength(256).WithMessage("GuestEmail must be at most 256 characters.");

            RuleFor(x => x.GuestPhone)
                .NotEmpty().WithMessage("GuestPhone is required for guest rentals.")
                .MaximumLength(30).WithMessage("GuestPhone must be at most 30 characters.");
        });

        When(x => x.UserId.HasValue, () =>
        {
            RuleFor(x => x.GuestName)
                .Must(string.IsNullOrWhiteSpace)
                .WithMessage("GuestName must be empty when UserId is provided.");

            RuleFor(x => x.GuestEmail)
                .Must(string.IsNullOrWhiteSpace)
                .WithMessage("GuestEmail must be empty when UserId is provided.");

            RuleFor(x => x.GuestPhone)
                .Must(string.IsNullOrWhiteSpace)
                .WithMessage("GuestPhone must be empty when UserId is provided.");
        });
    }
}