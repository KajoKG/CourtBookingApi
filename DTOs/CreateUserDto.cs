using CourtBooking.Api.Entities;

namespace CourtBooking.Api.DTOs;

public class CreateUserDto
{

    public string Name { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}