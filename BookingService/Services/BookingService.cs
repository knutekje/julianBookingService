using BookingService.Data;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Services;

public class BookingService : IBookingService
{
    private readonly BookingDbContext _context;
    private readonly IRoomServiceClient _roomServiceClient;

    public BookingService(BookingDbContext context, IRoomServiceClient roomService)
    {
        _context = context;
        _roomServiceClient = roomService;
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
        booking.CheckInDate = booking.CheckInDate.ToUniversalTime();
        booking.CheckOutDate = booking.CheckOutDate.ToUniversalTime();

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
    //        var rooms = await _roomServiceClient.GetAllRoomsAsync();

   /* public async Task<IEnumerable<string>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
    {
        // Fetch all rooms
        
        var rooms = await _roomServiceClient.GetAllRoomsAsync();
        var allRoomNumbers =  rooms.AsQueryable().Select(r => r.RoomNumber);

        // Fetch room numbers that are already booked for the given dates
        var bookedRoomNumbers = await _context.Bookings
            .Where(b => b.CheckOutDate > checkIn && b.CheckInDate < checkOut )
            .Select(b => b.RoomNumber)
            .ToListAsync();

        // Return rooms that are not booked during the given dates
        return allRoomNumbers.Except(bookedRoomNumbers);
    }*/
    
   public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
   {
       // Ensure the input dates are in UTC
       var normalizedCheckIn = DateTime.SpecifyKind(checkIn, DateTimeKind.Utc);
       var normalizedCheckOut = DateTime.SpecifyKind(checkOut, DateTimeKind.Utc);

       // Fetch all rooms from the RoomService
       var rooms = await _roomServiceClient.GetAllRoomsAsync();

       // Fetch room numbers that are already booked for the given dates
       var bookedRoomNumbers = await _context.Bookings
           .Where(b => b.CheckOutDate > normalizedCheckIn && b.CheckInDate < normalizedCheckOut && b.Status != "Out of Service")
           .Select(b => b.RoomNumber)
           .ToListAsync();

       // Return rooms that are not booked during the given dates
       return rooms.Where(r => !bookedRoomNumbers.Contains(r.RoomNumber));
   }
 

    
    
    /*
     * public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
{
    // Fetch all rooms from the database
    var allRooms = await _context.Rooms.ToListAsync();

    // Fetch room numbers that are already booked for the given dates
    var bookedRoomNumbers = await _context.Bookings
        .Where(b => b.CheckOutDate > checkIn && b.CheckInDate < checkOut && b.Status == "Confirmed")
        .Select(b => b.RoomNumber)
        .ToListAsync();

    // Return rooms that are not booked during the given dates
    return allRooms.Where(r => !bookedRoomNumbers.Contains(r.RoomNumber));
}

     */



    public async Task<bool> IsRoomOccupiedAsync(string roomNumber, DateTime date)
    {
        return await _context.Bookings.AnyAsync(b =>
            b.RoomNumber == roomNumber && b.CheckInDate <= date && b.CheckOutDate > date && b.Status == "Confirmed");
    }
    
    public async Task<bool> ConfirmBookingAsync(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return false;

        booking.Status = "Confirmed";
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> ValidateRoomForBooking(string roomNumber, DateTime checkIn, DateTime checkOut)
    {
        // Check for any existing bookings that overlap with the requested dates
        var hasConflictingBooking = await _context.Bookings.AnyAsync(b =>
                b.RoomNumber == roomNumber &&
                //b.Status == "Confirmed" &&
                b.CheckInDate < checkOut && // Existing booking starts before the requested check-out
                b.CheckOutDate > checkIn    // Existing booking ends after the requested check-in
        );

        // If there is a conflicting booking, the room is not available
        return !hasConflictingBooking;
    }


    
    
    

}