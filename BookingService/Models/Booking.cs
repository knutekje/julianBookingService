namespace BookingService.Models;

public class Booking
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string GuestName { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled
}