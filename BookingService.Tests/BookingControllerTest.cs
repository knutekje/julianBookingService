using BookingService.Controllers;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class BookingsControllerTests
{
    private readonly Mock<IBookingService> _mockBookingService;
    private readonly BookingsController _controller;

    public BookingsControllerTests()
    {
        _mockBookingService = new Mock<IBookingService>();
        _controller = new BookingsController(_mockBookingService.Object);
    }

    [Fact]
    public async Task GetAvailableRooms_ShouldReturnOkWithAvailableRooms()
    {
        var checkIn = DateTime.UtcNow.AddDays(1);
        var checkOut = DateTime.UtcNow.AddDays(3);
        var rooms = new List<RoomDTO>
        {
            new RoomDTO { Id = 1, RoomNumber = "101" },
            new RoomDTO { Id = 2, RoomNumber = "102" }
        };

        _mockBookingService
            .Setup(service => service.GetAvailableRoomsAsync(checkIn, checkOut))
            .ReturnsAsync(rooms);

        CheckRoomDTO checkRoomDto = new CheckRoomDTO
        {
            CheckIn = checkIn,
            CheckOut = checkOut,
        };
        var result = await _controller.GetAvailableRooms(checkRoomDto);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRooms = Assert.IsType<List<RoomDTO>>(okResult.Value);
        Assert.Equal(2, returnedRooms.Count);
    }

    [Fact]
    public async Task GetBookingById_ShouldReturnOkWithBooking_WhenExists()
    {
        var bookingId = 1;
        var booking = new Booking
        {
            Id = bookingId,
            GuestId = 1,
            RoomId = 1,
            CheckInDate = DateTime.UtcNow.AddDays(1),
            CheckOutDate = DateTime.UtcNow.AddDays(3)
        };

        _mockBookingService
            .Setup(service => service.GetBookingByIdAsync(bookingId))
            .ReturnsAsync(booking);

        var result = await _controller.GetBooking(bookingId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBooking = Assert.IsType<Booking>(okResult.Value);
        Assert.Equal(bookingId, returnedBooking.Id);
    }

    [Fact]
    public async Task GetBookingById_ShouldReturnNotFound_WhenBookingDoesNotExist()
    {
        var bookingId = 1;

        _mockBookingService
            .Setup(service => service.GetBookingByIdAsync(bookingId))
            .ReturnsAsync((Booking?)null);

        var result = await _controller.GetBooking(bookingId);

        Assert.IsType<NotFoundResult>(result.Result);
    }
}
