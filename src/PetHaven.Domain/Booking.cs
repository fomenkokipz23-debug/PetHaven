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
        // Залишаємо ArgumentNullException, оскільки це базова системна перевірка на null
        if (pet == null) throw new ArgumentNullException(nameof(pet));
        if (room == null) throw new ArgumentNullException(nameof(room));
        
        // Замінено на BusinessRuleException (Логіка предметної області)
        if (checkIn < DateTime.Today) 
            throw new BusinessRuleException("Дата заїзду не може бути в минулому.");
            
        if (checkOut <= checkIn) 
            throw new BusinessRuleException("Дата виїзду має бути пізнішою за дату заїзду.");
            
        if (room.IsOccupied) 
            throw new BusinessRuleException("Цей вольєр уже зайнятий.");

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
        // Замінено на BusinessRuleException (Захист від некоректних переходів статусів)
        if (Status != BookingStatus.Active) 
            throw new BusinessRuleException("Тільки активне бронювання можна завершити.");
            
        Status = BookingStatus.Completed;
        Room.Release();
    }

    public void CancelBooking()
    {
        // Замінено на BusinessRuleException (Захист від некоректних переходів статусів)
        if (Status != BookingStatus.Active) 
            throw new BusinessRuleException("Тільки активне бронювання можна скасувати.");
            
        Status = BookingStatus.Cancelled;
        Room.Release();
    }
}