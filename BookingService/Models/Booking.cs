namespace BookingService.Models;
public class Booking
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string GuestName { get; set; } = string.Empty;

    private DateTime _checkInDate;
    public DateTime CheckInDate
    {
        get => _checkInDate;
        set => _checkInDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    private DateTime _checkOutDate;
    public DateTime CheckOutDate
    {
        get => _checkOutDate;
        set => _checkOutDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled
}

