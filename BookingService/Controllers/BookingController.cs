using System.Globalization;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
    {
        var bookings = await _bookingService.GetAllBookingsAsync();
        return Ok(bookings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Booking>> GetBooking(int id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);
        if (booking == null) return NotFound();
        return Ok(booking);
    }

    [HttpPost]
    public async Task<ActionResult<Booking>> PostBooking(Booking booking)
    {
        var isValidRoom = await _bookingService.ValidateRoomAvailabilityForBookingAsync(booking.RoomId, booking.CheckInDate, booking.CheckOutDate);

        if (!isValidRoom)
        {
            return BadRequest("The selected room is not available for the specified dates.");
        }

        var createdBooking = await _bookingService.CreateBookingAsync(booking);
        return CreatedAtAction(nameof(GetBooking), new { id = createdBooking.Id }, createdBooking);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutBooking(int id, Booking updatedBooking)
    {
        if (updatedBooking.CheckInDate >= updatedBooking.CheckOutDate)
        {
            return BadRequest("Check-in date must be earlier than check-out date.");
        }

        var isRoomValid = await _bookingService.ValidateRoomAvailabilityForBookingAsync(
            updatedBooking.RoomId, updatedBooking.CheckInDate, updatedBooking.CheckOutDate);

        if (!isRoomValid)
        {
            return BadRequest("The selected room is not available for the specified dates.");
        }

        var booking = await _bookingService.UpdateBookingAsync(id, updatedBooking);
        if (booking == null)
        {
            return NotFound($"No booking found with ID {id}.");
        }

        return Ok(booking);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var success = await _bookingService.DeleteBookingAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
    
    [HttpPost("available-rooms")]
    public async Task<ActionResult<IEnumerable<RoomDTO>>> GetAvailableRooms(CheckRoomDTO checkRoom)
    {
        if (checkRoom.CheckIn >= checkRoom.CheckOut)
        {
            return BadRequest("Check-in date must be earlier than check-out date.");
        }

        var availableRooms = await _bookingService.GetAvailableRoomsAsync(checkRoom.CheckIn, checkRoom.CheckOut);

        if (!availableRooms.Any())
        {
            return NotFound("No available rooms for the selected dates.");
        }


        return Ok(availableRooms);
    }


    
}