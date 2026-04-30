namespace CourtBooking.Api.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public UserRole Role { get; set; }
}