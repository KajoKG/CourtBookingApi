using Carter;
using CourtBooking.Api.Data;
using CourtBooking.Api.DTOs;
using CourtBooking.Api.Entities;

namespace CourtBooking.Api.Modules;

public class UserModule : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // Prikaz svih registriranih korisnika
        app.MapGet("/users", (int actorUserId, AppDbContext db) =>
        {
            var actor = db.Users.Find(actorUserId);

            if (actor is null)
                return Results.BadRequest("Invalid actor");

            if (actor.Role != UserRole.Admin)
                return Results.BadRequest("You don't have permission");

            return Results.Ok(db.Users.ToList());
        });

        // Stvaranje novog korisnika
        app.MapPost("/users", (CreateUserDto dto, AppDbContext db) =>
        {
            var user = new User
            {
                Name = dto.Name,
                Role = dto.Role
            };

            db.Users.Add(user);
            db.SaveChanges();

            return Results.Created($"/users/{user.Id}", user);
        });

        // Brisanje korisnika
        app.MapDelete("/users/{id}", (int id, int actorUserId, AppDbContext db) =>
        {
            var actor = db.Users.Find(actorUserId);

            if (actor is null)
                return Results.BadRequest("Invalid actor");

            if (actor.Role != UserRole.Admin)
                return Results.BadRequest("You don't have permission");

            var user = db.Users.Find(id);
            if (user is null)
                return Results.NotFound();

            db.Users.Remove(user);
            db.SaveChanges();

            return Results.NoContent();
        });
    }
}