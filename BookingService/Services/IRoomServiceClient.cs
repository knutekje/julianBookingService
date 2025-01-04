
using BookingService.Models;

namespace BookingService.Services;

public interface IRoomServiceClient
{
    Task<IEnumerable<Room>> GetAllRoomsAsync();
    Task<Room?> GetRoomByNumberAsync(string roomNumber);
}
