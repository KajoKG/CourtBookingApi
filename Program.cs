using Carter;
using CourtBooking.Api.Data;
using CourtBooking.Api.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Generiranje swagger UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registracija Cartera
builder.Services.AddCarter();

// Povezivanje aplikacije s bazom
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User { Name = "Admin", Role = UserRole.Admin },
            new User { Name = "Ivan", Role = UserRole.User },
            new User { Name = "Marko", Role = UserRole.User }
        );
    }

    if (!db.Courts.Any())
    {
        db.Courts.AddRange(
            new Court { Name = "Court 1", SportType = "tennis" },
            new Court { Name = "Court 2", SportType = "football" },
            new Court { Name = "Court 3", SportType = "tennis" },
            new Court { Name = "Court 4", SportType = "football" },
            new Court { Name = "Court 5", SportType = "basketball" }
        );
    }

    db.SaveChanges();
}

app.MapCarter();

app.Run();