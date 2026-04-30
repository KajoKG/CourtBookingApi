using Carter;
using CourtBooking.Api.Data;
using CourtBooking.Api.DTOs;
using CourtBooking.Api.Entities;

namespace CourtBooking.Api.Modules;

public class CourtModule : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // Prikaz terena s opcionalnim filterom za sport
        app.MapGet("/courts", (string? sportType, AppDbContext db) =>
        {
            var courts = db.Courts.AsQueryable();

            if (!string.IsNullOrEmpty(sportType))
            {
                var normalized = sportType.ToLower();
                courts = courts.Where(c => c.SportType == normalized);
            }

            return Results.Ok(courts.ToList());
        });

        // Stvaranje terena
        app.MapPost("/courts", (CreateCourtDto dto, AppDbContext db) =>
        {
            var actor = db.Users.Find(dto.ActorUserId);

            if (actor is null)
                return Results.BadRequest("Invalid actor");

            if (actor.Role != UserRole.Admin)
                return Results.BadRequest("You don't have permission");

            var allowedSports = new[] { "tennis", "basketball", "football" };
            var sport = dto.SportType.ToLower();

            if (!allowedSports.Contains(sport))
                return Results.BadRequest("Invalid sport type");

            var court = new Court
            {
                Name = dto.Name,
                SportType = sport
            };

            db.Courts.Add(court);
            db.SaveChanges();

            return Results.Created($"/courts/{court.Id}", court);
        });

        // Brisnaje terena
        app.MapDelete("/courts/{id}", (int id, int actorUserId, AppDbContext db) =>
        {
            var actor = db.Users.Find(actorUserId);

            if (actor is null)
                return Results.BadRequest("Invalid actor");

            if (actor.Role != UserRole.Admin)
                return Results.BadRequest("You don't have permission");

            var court = db.Courts.Find(id);
            if (court is null)
                return Results.NotFound();

            db.Courts.Remove(court);
            db.SaveChanges();

            return Results.NoContent();
        });

        // Dostupnost terena
        app.MapGet("/courts/{id}/availability", (int id, DateTime date, AppDbContext db) =>
        {
            var court = db.Courts.Find(id);

            if (court is null)
                return Results.NotFound("Court not found");

            var bookings = db.Bookings
                .Where(b => b.CourtId == id && b.StartTime.Date == date.Date)
                .ToList();

            var availableSlots = new List<string>();

            for (int hour = 7; hour < 23; hour++)
            {
                var start = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0);
                var end = start.AddHours(1);

                var isTaken = bookings.Any(b =>
                    start < b.EndTime &&
                    end > b.StartTime
                );

                if (!isTaken)
                {
                    availableSlots.Add($"{hour}:00 - {hour + 1}:00");
                }
            }

            return Results.Ok(availableSlots);
        });
    }
}