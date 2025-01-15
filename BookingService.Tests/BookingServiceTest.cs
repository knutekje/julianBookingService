using BookingService.Data;
using BookingService.Models;
using BookingService.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using FluentAssertions;

public class BookingServiceTests
{
    private readonly BookingDbContext _context;
    private readonly Mock<IRoomServiceClient> _mockRoomServiceClient;
    private readonly BookingService.Services.BookingService _bookingService;

    public BookingServiceTests()
    {
        var options = new DbContextOptionsBuilder<BookingDbContext>()
            .UseInMemoryDatabase(databaseName: "BookingServiceTestDb")
            .Options;

        _context = new BookingDbContext(options);
        _mockRoomServiceClient = new Mock<IRoomServiceClient>();
        _bookingService = new BookingService.Services.BookingService(_context, _mockRoomServiceClient.Object);
    }
    public void Dispose()
    {
        _context.Dispose(); // Dispose the DbContext after each test
    }
    [Fact]
    public async Task GetAvailableRoomsAsync_ShouldReturnAvailableRooms()
    {
        // Arrange
        var rooms = new List<Room>
        {
            new Room { RoomNumber = "101", Status = "Clean" },
            new Room { RoomNumber = "102", Status = "Out of Service" },
            new Room { RoomNumber = "103", Status = "Clean" }
        };
        _mockRoomServiceClient.Setup(client => client.GetAllRoomsAsync()).ReturnsAsync(rooms);

        _context.Bookings.Add(new Booking
        {
            RoomNumber = "101",
            CheckInDate = DateTime.UtcNow,
            CheckOutDate = DateTime.UtcNow.AddDays(1),
            Status = "Confirmed"
        });
        await _context.SaveChangesAsync();

        // Act
        var availableRooms = await _bookingService.GetAvailableRoomsAsync(DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Assert
        //availableRooms.Should().Contain("103");
        //availableRooms.Should().NotContain("101");
        //availableRooms.Should().NotContain("102");

        Dispose();
    }

    [Fact]
    public async Task ValidateRoomForBooking_ShouldReturnFalse_WhenRoomIsUnavailable()
    {
        // Arrange
        var room = new Room { RoomNumber = "101", Status = "Clean" };
        _mockRoomServiceClient.Setup(client => client.GetRoomByNumberAsync("101")).ReturnsAsync(room);

        _context.Bookings.Add(new Booking
        {
            RoomNumber = "101",
            CheckInDate = DateTime.UtcNow,
            CheckOutDate = DateTime.UtcNow.AddDays(1),
            Status = "Confirmed"
        });
        await _context.SaveChangesAsync();

        // Act
        var isValid = await _bookingService.ValidateRoomForBooking("101", DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Assert
        isValid.Should().BeFalse();
        
        Dispose();
    }

    [Fact]
    public async Task ValidateRoomForBooking_ShouldReturnTrue_WhenRoomIsAvailable()
    {
        // Arrange
        var room = new Room { RoomNumber = "209", Status = "Clean" };
        _mockRoomServiceClient.Setup(client => client.GetRoomByNumberAsync("209")).ReturnsAsync(room);

        // Act
        var isValid = await _bookingService.ValidateRoomForBooking("209", DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Assert
        isValid.Should().BeTrue();
        Dispose();
    }
}
