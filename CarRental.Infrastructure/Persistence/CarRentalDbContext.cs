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
    //kitoroltem a tesztadatokat hogy ne legyen utkozes illetve az efbol is kilett bombazva, szoval elvileg nem dob hibat
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CarRentalDbContext).Assembly);
        base.OnModelCreating(modelBuilder);

    }
}