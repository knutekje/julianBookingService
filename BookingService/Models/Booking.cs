namespace BookingService.Models;

public class Booking
{
    public int Id { get; set; } 
    public int RoomId { get; set; } 
    public int GuestId { get; set; } 
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
    public bool CheckedIn { get; set; } = false; 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    
}