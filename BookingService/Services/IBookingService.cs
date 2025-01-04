using BookingService.Models;

namespace BookingService.Services;

public interface IBookingService
{
    Task<IEnumerable<Booking>> GetAllBookingsAsync();
    Task<Booking?> GetBookingByIdAsync(int id);
    Task<Booking> CreateBookingAsync(Booking booking);
    Task<bool> DeleteBookingAsync(int id);
    Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);
    Task<bool> IsRoomOccupiedAsync(string roomNumber, DateTime date);
    Task<bool> ValidateRoomForBooking(string bookingRoomNumber, DateTime bookingCheckInDate, DateTime bookingCheckOutDate);
    Task<Booking?> UpdateBookingAsync(int id, Booking updatedBooking);
}
