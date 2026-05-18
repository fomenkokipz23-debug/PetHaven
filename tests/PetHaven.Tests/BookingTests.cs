using Xunit;
using PetHaven.Domain;
using System;

namespace PetHaven.Tests;

public class BookingTests
{
    [Fact]
    public void Constructor_ValidData_CalculatesCorrectTotalPrice()
    {
        var pet = new Pet("Рекс", PetType.Dog, 3);
        var room = new Room("101", RoomType.Standard, 400);
        var checkIn = DateTime.Today;
        var checkOut = DateTime.Today.AddDays(5);

        var booking = new Booking(pet, room, checkIn, checkOut);

        Assert.Equal(2000, booking.TotalPrice);
    }

    [Fact]
    public void Constructor_InvalidDates_ThrowsArgumentException()
    {
        var pet = new Pet("Мурка", PetType.Cat, 2);
        var room = new Room("102", RoomType.Comfort, 500);
        var checkIn = DateTime.Today;
        var checkOut = DateTime.Today.AddDays(-1); 

        Assert.Throws<ArgumentException>(() => new Booking(pet, room, checkIn, checkOut));
    }
}