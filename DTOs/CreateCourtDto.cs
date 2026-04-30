namespace CourtBooking.Api.DTOs;

public class CreateCourtDto
{
    public int ActorUserId { get; set; } 

    public string Name { get; set; } = string.Empty;
    public string SportType { get; set; } = string.Empty;
}