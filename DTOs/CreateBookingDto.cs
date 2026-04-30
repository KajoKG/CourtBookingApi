namespace CourtBooking.Api.DTOs;

public class CreateBookingDto
{
    public int ActorUserId { get; set; }

    public int CourtId { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}