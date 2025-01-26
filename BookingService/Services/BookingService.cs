using System.Collections;
using System.Runtime.InteropServices;
using BookingService.Data;
using BookingService.DTOs;
using BookingService.External;
using BookingService.Models;

namespace BookingService.Services;


using Microsoft.EntityFrameworkCore;

public class BookingService : IBookingService
{
    private readonly BookingDbContext _context;
    private readonly IGuestServiceClient _guestServiceClient;
    private readonly IRoomServiceClient _roomServiceClient;

    public BookingService(BookingDbContext context, IGuestServiceClient guestServiceClient, IRoomServiceClient roomServiceClient)
    {
        _context = context;
        _guestServiceClient = guestServiceClient;
        _roomServiceClient = roomServiceClient;
    }

    public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
    {
        return await _context.Bookings.ToListAsync();    }

    public async Task<Booking?> GetBookingByIdAsync(int id)
    {
        return await   _context.Bookings.FindAsync(id);    
    }

    public async Task<Booking> CreateBookingAsync(Booking booking)
    {
        if (!await ValidateRoomAvailabilityForBookingAsync(booking.RoomId, booking.CheckInDate, booking.CheckOutDate))
        {
            throw new InvalidOperationException("Room is not available for the selected dates.");
        }

        _context.Bookings.Add(booking);

      
        for (var date = booking.CheckInDate; date < booking.CheckOutDate; date = date.AddDays(1))
        {
            _context.BookedRooms.Add(new BookedRoom
            {
                RoomId = booking.RoomId,
                DateBooked = date,
                BookingId = booking.Id
            });
        }

        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<bool> DeleteBookingAsync(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return false;

        _context.Bookings.Remove(booking);

      
        var bookedRooms = _context.BookedRooms.Where(br => br.BookingId == id);
        _context.BookedRooms.RemoveRange(bookedRooms);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Booking?> UpdateBookingAsync(int id, Booking updatedBooking)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return null;

        if (!await ValidateRoomAvailabilityForBookingAsync(updatedBooking.RoomId, updatedBooking.CheckInDate, updatedBooking.CheckOutDate))
        {
            throw new InvalidOperationException("Room is not available for the updated dates.");
        }

        booking.RoomId = updatedBooking.RoomId;
        booking.CheckInDate = updatedBooking.CheckInDate;
        booking.CheckOutDate = updatedBooking.CheckOutDate;
        booking.CheckedIn = updatedBooking.CheckedIn;
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<IEnumerable<RoomDTO>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
    {
        var bookedRoomIds = await _context.BookedRooms
            .Where(br => br.DateBooked >= checkIn && br.DateBooked < checkOut)
            .Select(br => br.RoomId)
            .Distinct()
            .ToListAsync();

        return await _roomServiceClient.GetRoomsExcludingIdsAsync(bookedRoomIds);
    }



    private async Task<IEnumerable<int>> GetBookedRoomIdsAsync(DateTime checkIn, DateTime checkOut)
    {
        return await _context.BookedRooms
            .Where(br => br.DateBooked >= checkIn && br.DateBooked < checkOut)
            .Select(br => br.RoomId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        var isBooked = await _context.BookedRooms
            .AnyAsync(br => br.RoomId == roomId && br.DateBooked >= checkIn && br.DateBooked < checkOut);

        return !isBooked;
    }

    public async Task<bool> ValidateRoomAvailabilityForBookingAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        var room = await _roomServiceClient.GetRoomByIdAsync(roomId);
        if (room == null)
        {
            throw new KeyNotFoundException($"Room with ID {roomId} does not exist.");
        }

        return await IsRoomAvailableAsync(roomId, checkIn, checkOut);
    }
}
