using CarRental.Infrastructure.Persistence;
using CarRental.Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using CarRental.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure();

builder.Services.AddDbContext<CarRentalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();
    await dbContext.Database.MigrateAsync();

    var initializer = scope.ServiceProvider.GetRequiredService<IAppDbInitializer>();
    await initializer.InitializeAsync();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();