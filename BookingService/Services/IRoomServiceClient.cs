using BookingService.DTOs;

namespace BookingService.External
{
    public interface IRoomServiceClient
    {
        Task<RoomDTO?> GetRoomByIdAsync(int id);
        Task<RoomDTO?> GetRoomByRoomNumberAsync(string roomNumber);
        Task<IEnumerable<RoomDTO>> GetAllRoomsAsync();
        Task<IEnumerable<RoomDTO>> GetDirtyRoomsAsync();
        Task<IEnumerable<RoomDTO>> GetRoomsExcludingIdsAsync(IEnumerable<int> excludedRoomIds);
    }
}