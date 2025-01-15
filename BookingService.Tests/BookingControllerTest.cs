using BookingService.Controllers;
using BookingService.Models;
using BookingService.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;

public class BookingsControllerTests
{
    private readonly Mock<IBookingService> _mockBookingService;
    private readonly BookingsController _controller;

    public BookingsControllerTests()
    {
        _mockBookingService = new Mock<IBookingService>();
        _controller = new BookingsController(_mockBookingService.Object);
    }

    /*[Fact]
    public async Task GetAvailableRooms_ShouldReturnOkWithAvailableRooms()
    {
        // Arrange
        var availableRooms = new List<string> { "101", "102" };
        _mockBookingService.Setup(service => service.GetAvailableRoomsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(availableRooms);

        // Act
        var result = await _controller.GetAvailableRooms(DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(availableRooms);
    }*/

    [Fact]
    public async Task PostBooking_ShouldReturnBadRequest_WhenRoomIsUnavailable()
    {
        // Arrange
        var booking = new Booking
        {
            RoomNumber = "101",
            GuestName = "John Doe",
            CheckInDate = DateTime.UtcNow,
            CheckOutDate = DateTime.UtcNow.AddDays(1)
        };

        _mockBookingService.Setup(service => service.ValidateRoomForBooking(booking.RoomNumber, booking.CheckInDate, booking.CheckOutDate))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.PostBooking(booking);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task PostBooking_ShouldReturnCreated_WhenRoomIsAvailable()
    {
        // Arrange
        var booking = new Booking
        {
            RoomNumber = "101",
            GuestName = "John Doe",
            CheckInDate = DateTime.UtcNow,
            CheckOutDate = DateTime.UtcNow.AddDays(1)
        };

        _mockBookingService.Setup(service => service.ValidateRoomForBooking(booking.RoomNumber, booking.CheckInDate, booking.CheckOutDate))
            .ReturnsAsync(true);

        _mockBookingService.Setup(service => service.CreateBookingAsync(booking))
            .ReturnsAsync(booking);

        // Act
        var result = await _controller.PostBooking(booking);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Value.Should().BeEquivalentTo(booking);
    }
    
    [Fact]
    public async Task PutBooking_ShouldReturnOk_WhenUpdateIsSuccessful()
    {
        // Arrange
        var booking = new Booking
        {
            Id = 1,
            RoomNumber = "101",
            GuestName = "Jane Doe",
            CheckInDate = DateTime.UtcNow,
            CheckOutDate = DateTime.UtcNow.AddDays(1),
            Status = "Pending"
        };

        _mockBookingService.Setup(service => service.ValidateRoomForBooking(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(true);

        _mockBookingService.Setup(service => service.UpdateBookingAsync(1, It.IsAny<Booking>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _controller.PutBooking(1, booking);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(booking);
    }
    
    [Fact]
    public async Task PutBooking_ShouldReturnBadRequest_WhenDatesAreInvalid()
    {
        // Arrange
        var booking = new Booking
        {
            Id = 1,
            RoomNumber = "101",
            GuestName = "Jane Doe",
            CheckInDate = DateTime.UtcNow.AddDays(1),
            CheckOutDate = DateTime.UtcNow,
            Status = "Pending"
        };

        // Act
        var result = await _controller.PutBooking(1, booking);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
    [Fact]
    public async Task PutBooking_ShouldReturnBadRequest_WhenRoomIsUnavailable()
    {
        // Arrange
        var booking = new Booking
        {
            Id = 1,
            RoomNumber = "101",
            GuestName = "Jane Doe",
            CheckInDate = DateTime.UtcNow,
            CheckOutDate = DateTime.UtcNow.AddDays(1),
            Status = "Pending"
        };

        _mockBookingService.Setup(service => service.ValidateRoomForBooking(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.PutBooking(1, booking);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }


}
