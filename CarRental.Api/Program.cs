using CarRental.Application.Users;
using CarRental.Infrastructure;
using CarRental.Infrastructure.Managers;
using CarRental.Infrastructure.Persistence;
using CarRental.Infrastructure.Persistence.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure();

builder.Services.AddScoped<IUserManager, UserManager>();

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtIssuer = jwtSection["Issuer"] ?? "CarRental.Api";
var jwtAudience = jwtSection["Audience"] ?? "CarRental.Frontend";
var jwtKey = jwtSection["Key"] ?? "ChangeThisDevelopmentOnlyJwtKey123!";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<CarRentalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//egyberaktam a 3 Service.AddControllert
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                     ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

//Local: http://localhost:5173/ itt erheto el a cucc
//ha nem mukodne akkor a developer shellbe kell beirni: cd frontend, npm install majd npm run dev,
//ha UnathorizedAcces hiba van akkor pedig a win powershellbe kell ez a parancs:
//Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

//Set-ExecutionPolicy -ExecutionPolicy Undefined -Scope CurrentUser ezzel pedig vissza lehet allitani a powershellbe ezt az opciot(nem kotelezo)