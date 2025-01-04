namespace BookingService.Tests;

using BookingService.Data;
using BookingService.Models;
using BookingService.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

public class BookingServiceTests
{
    private readonly BookingDbContext _context;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        var options = new DbContextOptionsBuilder<BookingDbContext>()
            .UseInMemoryDatabase("BookingServiceTestDb")
            .Options;

        _context = new BookingDbContext(options);
        _bookingService = new BookingService(_context);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldAddBooking()
    {
        // Arrange
        var booking = new Booking
        {
            RoomNumber = "101",
            GuestName = "John Doe",
            CheckInDate = DateTime.Now,
            CheckOutDate = DateTime.Now.AddDays(2),
            Status = "Pending"
        };

        // Act
        var createdBooking = await _bookingService.CreateBookingAsync(booking);

        // Assert
        var dbBooking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == createdBooking.Id);
        dbBooking.Should().NotBeNull();
        dbBooking.GuestName.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetAllBookingsAsync_ShouldReturnBookings()
    {
        // Arrange
        _context.Bookings.Add(new Booking { RoomNumber = "101", GuestName = "Jane Doe", Status = "Confirmed" });
        _context.Bookings.Add(new Booking { RoomNumber = "102", GuestName = "John Smith", Status = "Pending" });
        await _context.SaveChangesAsync();

        // Act
        var bookings = await _bookingService.GetAllBookingsAsync();

        // Assert
        bookings.Should().HaveCount(2);
    }
}
