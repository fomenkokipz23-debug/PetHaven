using System;
using Xunit;
using PetHaven.Domain;

namespace PetHaven.Tests;

public class BookingTests
{
    [Fact]
    public void Constructor_ValidData_CalculatesCorrectTotalPrice()
    {
        // Arrange
        var pet = new Pet("Рекс", PetType.Dog, 3);
        var room = new Room("101", RoomType.Standard, 400);
        var checkIn = DateTime.Today;
        var checkOut = DateTime.Today.AddDays(5);
        var strategy = new RegularPricingStrategy(); // ДОДАТИ ЦЕ

        // Act (Додано strategy останнім параметром)
        var booking = new Booking(pet, room, checkIn, checkOut, strategy);

        // Assert
        Assert.Equal(2000, booking.TotalPrice);
    }

    [Fact]
    public void Constructor_InvalidDates_ThrowsArgumentException()
    {
        // Arrange
        var pet = new Pet("Мурка", PetType.Cat, 2);
        var room = new Room("102", RoomType.Comfort, 500);
        var checkIn = DateTime.Today;
        var checkOut = DateTime.Today.AddDays(-1);
        var strategy = new RegularPricingStrategy(); // ДОДАТИ ЦЕ

        // Act & Assert (Додано strategy останнім параметром)
        Assert.Throws<ArgumentException>(() => new Booking(pet, room, checkIn, checkOut, strategy));
    }
}