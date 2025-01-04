using BookingService.Data;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Services;

public class BookingService : IBookingService
{
    private readonly BookingDbContext _context;

    public BookingService(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
    {
        return await _context.Bookings.ToListAsync();
    }

    public async Task<Booking?> GetBookingByIdAsync(int id)
    {
        return await _context.Bookings.FindAsync(id);
    }

    public async Task<Booking> CreateBookingAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<Booking?> UpdateBookingAsync(int id, Booking updatedBooking)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return null;

        booking.RoomNumber = updatedBooking.RoomNumber;
        booking.GuestName = updatedBooking.GuestName;
        booking.CheckInDate = updatedBooking.CheckInDate;
        booking.CheckOutDate = updatedBooking.CheckOutDate;
        booking.Status = updatedBooking.Status;

        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<bool> DeleteBookingAsync(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return false;

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return true;
    }
}