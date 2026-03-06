using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Infrastructure.Persistence;

public class CarRentalDbContext : DbContext
{
    public CarRentalDbContext(DbContextOptions<CarRentalDbContext> options) : base(options) { }

    public DbSet<Users> Users => Set<Users>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Rental> Rentals => Set<Rental>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CarRentalDbContext).Assembly);
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Users>()
            .HasMany(p => p.Roles)
            .WithMany(p => p.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserRoles",
                j => j.HasOne<Role>().WithMany().HasForeignKey("RolesId"),
                j => j.HasOne<Users>().WithMany().HasForeignKey("UsersId"),
                j => {
                    j.HasKey("RolesId", "UsersId");
                    j.ToTable("UserRoles");
                });

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, RoleType = RoleTypes.Admin },
            new Role { Id = 2, RoleType = RoleTypes.Officer },
            new Role { Id = 3, RoleType = RoleTypes.Customer }
        );

        modelBuilder.Entity<Users>().HasData(
            new Users
            {
                Id = 1,
                UserName = "admin",
                Email = "admin@admin.hu",
                Password = "admin123",
                FirstName = "Admin",
                LastName = "Admin",
                Address = "Admin Street 1",
                Phone = "+36206766767"


            },
            new Users
            {
                Id = 2,
                UserName = "VadonJani67",
                Email = "vadjanos67@freemail.hu",
                Password = "viccesjelszo",
                FirstName = "Vad",
                LastName = "János",
                Address = "Haszkovó u. 67",
                Phone = "+36306766767"


            },

            new Users
            {
                Id = 3,
                UserName = "DomToretto",
                Email = "acsaladazelso@csalad.com",
                Password = "csalad4ever",
                FirstName = "Dom",
                LastName = "Toretto",
                Address = "Fast Street 1",
                Phone = "+3612345678"

            },
             new Users
             {
                 Id = 4,
                 UserName = "JohnPork13",
                 Email = "johnp13@gmail.com",
                 Password = "tehenhus42",
                 FirstName = "Sertés",
                 LastName = "János",
                 Address = "Vágó utca 13",
                 Phone = "+36706766767"

             }
        );

        modelBuilder.Entity<Car>().HasData(
        new Car
        {
            Id = 1,
            Brand = "Toyota",
            Model = "Corolla",
            LicensePlate = "AA-BB-123",
            DistanceKm = 110000,
            DailyPrice = 15000,
            Status = CarStatus.Available
        },
        new Car
        {
            Id = 2,
            Brand = "Tesla",
            Model = "Model 3",
            LicensePlate = "EL-ON-420",
            DistanceKm = 23000,
            DailyPrice = 45000,
            Status = CarStatus.Available
        },
        new Car
        {
            Id = 3,
            Brand = "Ford",
            Model = "Mondeo",
            LicensePlate = "SIX-767",
            DistanceKm = 180000,
            DailyPrice = 8000,
            Status = CarStatus.Unavailable
        }
        );

        modelBuilder.Entity<Rental>().HasData(
            new Rental
            {
                Id = 1,
                CarId = 1,
                UserId = 2,
                StartDate = new DateTime(2024, 7, 1),
                EndDate = new DateTime(2024, 7, 5),
                Status = CarRentStatus.Requested

            },
            new Rental
            {
                Id = 2,
                CarId = 3,
                UserId = 4,
                StartDate = new DateTime(2024, 7, 10),
                EndDate = new DateTime(2024, 7, 15),
                Status = CarRentStatus.Requested
            }
        );
        modelBuilder.Entity("UserRoles").HasData(

            new { RolesId = 1, UsersId = 1 },
            new { RolesId = 2, UsersId = 2 },
            new { RolesId = 3, UsersId = 3 },
            new { RolesId = 2, UsersId = 4 }
            );
    }
}