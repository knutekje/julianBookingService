namespace BookingService.DTOs;

public class RoomDTO
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty; 
    public string RoomType { get; set; } = string.Empty;
    public int Capacity { get; set; } 
    public decimal Price { get; set; } 
    public RoomStatus Status { get; set; } 
    public bool ExtraBed { get; set; } // Indicates if the room can have an extra bed
    public DateTime? LastMaintenance { get; set; } 
}

public enum RoomStatus
{
    Clean,
    Dirty,
    OutOfService
}
