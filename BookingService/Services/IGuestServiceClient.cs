using BookingService.DTOs;

public interface IGuestServiceClient
{
    Task<GuestDTO?> GetGuestByIdAsync(int id);
    Task<IEnumerable<GuestDTO>> GetGuestsByIdsAsync(IEnumerable<int> guestIds);
}