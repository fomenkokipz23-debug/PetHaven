using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using PetHaven.Application;
using PetHaven.Domain;

namespace PetHaven.Tests;

public class UnitTests
{
    // --- Тести інваріантів доменних сутностей та граничних значень ---

    [Theory]
    [InlineData("Бакс", PetType.Dog, -1)]
    [InlineData("Мурчик", PetType.Cat, -2)]
    public void Pet_Constructor_InvalidAge_ThrowsBusinessRuleException(string name, PetType type, int age)
    {
        Assert.Throws<BusinessRuleException>(() => new Pet(name, type, age));
    }

    [Fact]
    public void Booking_Constructor_CheckOutBeforeCheckIn_ThrowsBusinessRuleException()
    {
        var pet = new Pet("Шарик", PetType.Dog, 3);
        var room = new Room("101", RoomType.Standard, 300);
        
        Assert.Throws<BusinessRuleException>(() => 
            new Booking(pet, room, DateTime.Today, DateTime.Today.AddDays(-1), new RegularPricingStrategy()));
    }

    [Fact]
    public void Booking_CancelCompletedBooking_ThrowsBusinessRuleException()
    {
        // Arrange
        var pet = new Pet("Рекс", PetType.Dog, 2);
        var room = new Room("102", RoomType.Standard, 300);
        var booking = new Booking(pet, room, DateTime.Today, DateTime.Today.AddDays(2), new RegularPricingStrategy());
        
        booking.CompleteBooking();
        
        // Act & Assert (Змінено на BusinessRuleException)
        Assert.Throws<BusinessRuleException>(() => booking.CancelBooking());
    }

    // --- Тести доменних сервісів із використанням Moq ---

    [Fact]
    public async Task BookRoomAsync_RoomIsOccupied_ThrowsBusinessRuleException()
    {
        // Arrange
        var mockRoomRepo = new Mock<IRoomRepository>();
        var mockBookingRepo = new Mock<IBookingRepository>();

        var occupiedRoom = new Room("201", RoomType.Luxury, 500);
        occupiedRoom.MarkAsOccupied();

        mockRoomRepo.Setup(r => r.GetByNumber("201")).Returns(occupiedRoom);

        var service = new BookingService(mockBookingRepo.Object, mockRoomRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() => 
            service.BookRoomAsync("Лео", PetType.Cat, 3, "201", 5, new RegularPricingStrategy()));
    }

    [Fact]
    public void AnalyticsService_GetTotalRevenue_CalculatesCorrectSum()
    {
        // Arrange
        var mockBookingRepo = new Mock<IBookingRepository>();
        var pet = new Pet("Кеня", PetType.Cat, 1);
        var room = new Room("101", RoomType.Standard, 100);
        
        var bookings = new List<Booking>
        {
            new Booking(pet, room, DateTime.Today, DateTime.Today.AddDays(2), new RegularPricingStrategy()), // 200
            new Booking(pet, room, DateTime.Today, DateTime.Today.AddDays(3), new RegularPricingStrategy())  // 300
        };

        mockBookingRepo.Setup(b => b.GetAll()).Returns(bookings);
        var analytics = new AnalyticsService(mockBookingRepo.Object);

        // Act
        var revenue = analytics.GetTotalRevenue();

        // Assert
        Assert.Equal(500m, revenue);
    }
}