namespace BookingService.DTOs;

public class GuestDTO
{
    public int Id { get; set; } // Guest ID
    public string FirstName { get; set; } = string.Empty; // First name
    public string LastName { get; set; } = string.Empty; // Last name
    public string Email { get; set; } = string.Empty; // Email address
    public string PhoneNumber { get; set; } = string.Empty; // Phone number
    public string? Address { get; set; } = null; // Address (optional)
}