namespace CourtBooking.Api.Entities;

public class Court
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string SportType { get; set; } = string.Empty;
}