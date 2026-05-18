using System;
using System.Collections.Generic;
using System.Linq; // Додано для роботи розширень LINQ (.FirstOrDefault)
using System.Threading.Tasks;
using PetHaven.Domain;

namespace PetHaven.Application;

public class BookingService
{
    // Використовуємо інтерфейси замість конкретних файлових класів для SOLID архітектури
    private readonly IBookingRepository _bookingRepo;
    private readonly IRoomRepository _roomRepo;

    public BookingService(IBookingRepository bookingRepo, IRoomRepository roomRepo)
    {
        _bookingRepo = bookingRepo;
        _roomRepo = roomRepo;
    }

    // UC-1: Заселення (З підтримкою патерну Strategy)
    public async Task<Booking> BookRoomAsync(string petName, PetType type, int age, string roomNumber, int days, IPricingStrategy pricingStrategy)
    {
        var room = _roomRepo.GetByNumber(roomNumber);
        
        // Замінено на BusinessRuleException
        if (room == null) 
            throw new BusinessRuleException("Кімнату не знайдено.");
            
        if (room.IsOccupied) 
            throw new BusinessRuleException("Цей вольєр уже зайнятий іншою твариною.");

        var pet = new Pet(petName, type, age);
        var booking = new Booking(pet, room, DateTime.Today, DateTime.Today.AddDays(days), pricingStrategy);

        room.MarkAsOccupied();
        _roomRepo.Update(room);
        _bookingRepo.Add(booking);

        // Зберігаємо зміни у файли асинхронно через контракти інтерфейсів
        await _roomRepo.SaveChangesAsync();
        await _bookingRepo.SaveChangesAsync();

        return booking;
    }

    // UC-2: Виселення (Check-Out)
    public async Task CompleteBookingAsync(Guid bookingId)
    {
        var bookings = _bookingRepo.GetAll();
        
        // Замінено на BusinessRuleException
        var booking = bookings.FirstOrDefault(b => b.Id == bookingId) 
            ?? throw new BusinessRuleException("Бронювання не знайдено.");

        booking.CompleteBooking();
        _roomRepo.Update(booking.Room);

        await _roomRepo.SaveChangesAsync();
        await _bookingRepo.SaveChangesAsync();
    }

    // UC-3: Скасування бронювання
    public async Task CancelBookingAsync(Guid bookingId)
    {
        var bookings = _bookingRepo.GetAll();
        
        // Замінено на BusinessRuleException
        var booking = bookings.FirstOrDefault(b => b.Id == bookingId) 
            ?? throw new BusinessRuleException("Бронювання не знайдено.");

        booking.CancelBooking();
        _roomRepo.Update(booking.Room);

        await _roomRepo.SaveChangesAsync();
        await _bookingRepo.SaveChangesAsync();
    }
}