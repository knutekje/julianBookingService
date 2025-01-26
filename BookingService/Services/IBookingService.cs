using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.Services;



public interface IBookingService
{
    Task<IEnumerable<Booking>> GetAllBookingsAsync();
    Task<Booking?> GetBookingByIdAsync(int id);
    Task<Booking> CreateBookingAsync(Booking booking);
    Task<bool> DeleteBookingAsync(int id);
    Task<Booking?> UpdateBookingAsync(int id, Booking updatedBooking);
    Task<IEnumerable<RoomDTO>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);
    Task<bool> ValidateRoomAvailabilityForBookingAsync(int roomId, DateTime checkIn, DateTime checkOut);
}