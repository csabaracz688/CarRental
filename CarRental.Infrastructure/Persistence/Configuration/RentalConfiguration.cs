using CarRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Infrastructure.Persistence.Configurations;

public class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.Property(x => x.GuestName).HasMaxLength(150);
        builder.Property(x => x.GuestEmail).HasMaxLength(256);
        builder.Property(x => x.GuestPhone).HasMaxLength(30);

        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.StartDate).IsRequired();
        builder.Property(x => x.EndDate).IsRequired();

        builder.HasOne(r => r.User)
            .WithMany(u => u.Rentals)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ApprovedByUser)
            .WithMany(u => u.ApprovedRent)
            .HasForeignKey(r => r.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Car)
            .WithMany(c => c.Rentals)
            .HasForeignKey(r => r.CarId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Invoice)
            .WithOne(i => i.Rental)
            .HasForeignKey<Invoice>(i => i.RentalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}