using BookingService.Data;
using BookingService.DTOs;
using BookingService.External;
using BookingService.Models;
using BookingService.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

public class BookingServiceTests
{
    private readonly BookingDbContext _dbContext;
    private readonly Mock<IRoomServiceClient> _roomServiceClientMock;
    private readonly Mock<IGuestServiceClient> _guestServiceClientMock;
    private readonly BookingService.Services.BookingService _bookingService;

    public BookingServiceTests()
    {
        var options = new DbContextOptionsBuilder<BookingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;

        _dbContext = new BookingDbContext(options);

        _roomServiceClientMock = new Mock<IRoomServiceClient>();
        _guestServiceClientMock = new Mock<IGuestServiceClient>();

        _bookingService = new BookingService.Services.BookingService(
            _dbContext,
            _guestServiceClientMock.Object,
            _roomServiceClientMock.Object
        );
    }

    [Fact]
    public async Task GetAvailableRoomsAsync_ShouldReturnAvailableRooms()
    {
        var checkIn = DateTime.UtcNow.AddDays(1);
        var checkOut = DateTime.UtcNow.AddDays(5);
    
        _dbContext.BookedRooms.AddRange(new List<BookedRoom>
        
        {
            new BookedRoom { RoomId = 1, DateBooked = DateTime.UtcNow.AddDays(2) },
            new BookedRoom { RoomId = 2, DateBooked = DateTime.UtcNow.AddDays(3) }
        });
        await _dbContext.SaveChangesAsync();

        var bookedRoomIds = new List<int> { 1, 2 };

        var availableRooms = new List<RoomDTO>
        {
            new RoomDTO { Id = 3, RoomNumber = "103" },
            new RoomDTO { Id = 4, RoomNumber = "104" }
        };

        _roomServiceClientMock.Setup(client => client.GetRoomsExcludingIdsAsync(bookedRoomIds))
            .ReturnsAsync(availableRooms);

        var result = await _bookingService.GetAvailableRoomsAsync(checkIn, checkOut);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("103", result.First().RoomNumber);
        Assert.Equal("104", result.Last().RoomNumber);

        _roomServiceClientMock.Verify(client => client.GetRoomsExcludingIdsAsync(It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(bookedRoomIds))), Times.Once);
    }


    [Fact]
    public async Task GetBookingByIdAsync_ShouldReturnBooking_WhenExists()
    {
        // Arrange: Create a booking and add it to the in-memory database
        var bookingId = 1;
        var booking = new Booking
        {
            Id = bookingId,
            GuestId = 1,
            CheckInDate = DateTime.UtcNow.AddDays(1),
            CheckOutDate = DateTime.UtcNow.AddDays(3),
            RoomId = 1
        };

        await _dbContext.Bookings.AddAsync(booking);
        await _dbContext.SaveChangesAsync();        

        var result = await _bookingService.GetBookingByIdAsync(bookingId);

        Assert.NotNull(result);                     
        Assert.Equal(bookingId, result.Id);        
        Assert.Equal(booking.GuestId, result.GuestId); 
        Assert.Equal(booking.RoomId, result.RoomId);
    }
    
    

}
