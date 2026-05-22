using System;

namespace PetHaven.Domain;

public class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Pet Pet { get; set; } = null!;   // Додано = null!; щоб прибрати warning
    public Room Room { get; set; } = null!; // Додано = null!; щоб прибрати warning
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; private set; } = BookingStatus.Active;

    // Конструктор для створення вручну
    public Booking(Pet pet, Room room, DateTime checkIn, DateTime checkOut, IPricingStrategy pricingStrategy)
    {
        if (pet == null) throw new ArgumentNullException(nameof(pet));
        if (room == null) throw new ArgumentNullException(nameof(room));
        if (checkIn < DateTime.Today) throw new ArgumentException("Дата заїзду не може бути в минулому.");
        if (checkOut <= checkIn) throw new ArgumentException("Дата виїзду має бути пізнішою за дату заїзду.");
        if (room.IsOccupied) throw new InvalidOperationException("Цей вольєр уже зайнятий.");

        Pet = pet;
        Room = room;
        CheckInDate = checkIn;
        CheckOutDate = checkOut;
        
        int days = (CheckOutDate - CheckInDate).Days;
        TotalPrice = pricingStrategy.CalculatePrice(room.PricePerNight, days, pet.Type);
    }

    // Конструктор без параметрів для серіалізації JSON
    public Booking() { }

    public void CompleteBooking()
    {
        if (Status != BookingStatus.Active) throw new InvalidOperationException("Тільки активне бронювання можна завершити.");
        Status = BookingStatus.Completed;
        Room.Release();
    }

    public void CancelBooking()
    {
        if (Status != BookingStatus.Active) throw new InvalidOperationException("Тільки активне бронювання можна скасувати.");
        Status = BookingStatus.Cancelled;
        Room.Release();
    }
}