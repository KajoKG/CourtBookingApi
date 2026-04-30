using Carter;
using CourtBooking.Api.Data;
using CourtBooking.Api.DTOs;
using CourtBooking.Api.Entities;

namespace CourtBooking.Api.Modules;

public class BookingModule : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // Kreiranje bookinga
        app.MapPost("/bookings", (CreateBookingDto dto, AppDbContext db) =>
        {
            var actor = db.Users.Find(dto.ActorUserId);

            if (actor is null)
                return Results.BadRequest("Invalid actor");

            var court = db.Courts.Find(dto.CourtId);
            if (court is null)
                return Results.NotFound("Court not found");

            var start = dto.StartTime;
            var end = dto.EndTime;

            // Provjera ispravnosti unosa
            if (start.Date != end.Date)
                return Results.BadRequest("Same day only");

            if (start.Minute != 0 || end.Minute != 0)
                return Results.BadRequest("Full hours only");

            var duration = end - start;
            if (duration.TotalHours <= 0 || duration.TotalHours > 2)
                return Results.BadRequest("Max 2h");

            if (start.Hour < 7 || end.Hour > 23)
                return Results.BadRequest("07-23 only");

            //Peovjera preklapanja s drugim bookingom
            var overlap = db.Bookings.Any(b =>
                b.CourtId == dto.CourtId &&
                start < b.EndTime &&
                end > b.StartTime
            );

            if (overlap)
                return Results.BadRequest("Already booked");

            
            //Kreiranje ispravnog bookinga
            var booking = new Booking
            {
                CourtId = dto.CourtId,
                UserId = actor.Id,
                StartTime = start,
                EndTime = end
            };

            db.Bookings.Add(booking);
            db.SaveChanges();

            return Results.Created($"/bookings/{booking.Id}", booking);
        });

        // Brisanje bookinga
        app.MapDelete("/bookings/{id}", (int id, int actorUserId, AppDbContext db) =>
        {
            var actor = db.Users.Find(actorUserId);

            if (actor is null)
                return Results.BadRequest("Invalid actor");

            var booking = db.Bookings.Find(id);
            if (booking is null)
                return Results.NotFound();

        // Pravo ima samo admin ili onaj koji je bookirao

            if (booking.UserId != actor.Id && actor.Role != UserRole.Admin)
                return Results.BadRequest("You don't have permission");

            db.Bookings.Remove(booking);
            db.SaveChanges();

            return Results.NoContent();
        });

        // Prikaz bookinga za određenog korisnika (pravo samo admin i taj korisnik)
        app.MapGet("/users/{userId}/bookings", (int userId, int actorUserId, AppDbContext db) =>
        {
            var actor = db.Users.Find(actorUserId);

            if (actor is null)
                return Results.BadRequest("Invalid actor");

            if (actor.Id != userId && actor.Role != UserRole.Admin)
                return Results.BadRequest("You don't have permission");

            var bookings = db.Bookings
                .Where(b => b.UserId == userId)
                .ToList();

            return Results.Ok(bookings);
        });
    }
}