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
        var createdBooking = await _bookingService.CreateBookingAsync(booking);
        return CreatedAtAction(nameof(GetBooking), new { id = createdBooking.Id }, createdBooking);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutBooking(int id, Booking updatedBooking)
    {
        var booking = await _bookingService.UpdateBookingAsync(id, updatedBooking);
        if (booking == null) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var success = await _bookingService.DeleteBookingAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}