namespace BookingService.Models;

public class BookedRoom
{
    public int RoomId { get; set; }

    public DateTime _datebooked;
    public DateTime DateBooked {   get => _datebooked;
        set => _datebooked = DateTime.SpecifyKind(value, DateTimeKind.Utc); } 
    public int BookingId { get; set; } 

    public override bool Equals(object? obj)
    {
        if (obj is not BookedRoom other) return false;
        return RoomId == other.RoomId && DateBooked == other.DateBooked;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(RoomId, DateBooked);
    }
}