using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;
using PetHaven.Domain;

namespace PetHaven.Application;

/// <summary>
/// Сервіс прикладного шару, що оркеструє сценарії використання (Use Cases) системи бронювання готелю.
/// </summary>
public class BookingService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IRoomRepository _roomRepo;

    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="BookingService"/> через інверсію залежностей (DIP).
    /// </summary>
    /// <param name="bookingRepo">Абстракція сховища бронювань.</param>
    /// <param name="roomRepo">Абстракція сховища кімнат/вольєрів.</param>
    public BookingService(IBookingRepository bookingRepo, IRoomRepository roomRepo)
    {
        _bookingRepo = bookingRepo ?? throw new ArgumentNullException(nameof(bookingRepo));
        _roomRepo = roomRepo ?? throw new ArgumentNullException(nameof(roomRepo));
    }

    /// <summary>
    /// Асинхронно виконує сценарій заселення тварини (UC-1) з автоматичним розрахунком вартості за обраною стратегією.
    /// </summary>
    /// <param name="petName">Кличка свійської тварини.</param>
    /// <param name="type">Тип тварини (кішка, собака тощо).</param>
    /// <param name="age">Вік тварини (повинен бути в межах від 0 до 30).</param>
    /// <param name="roomNumber">Унікальний текстовий номер вольєра.</param>
    /// <param name="days">Тривалість проживання у днях.</param>
    /// <param name="pricingStrategy">Поліморфна стратегія розрахунку фінальної вартості.</param>
    /// <returns>Об'єкт створеного та зафіксованого бронювання.</returns>
    /// <exception cref="BusinessRuleException">Викидається, якщо кімнату не знайдено або вольєр уже зарезервовано.</exception>
    public async Task<Booking> BookRoomAsync(string petName, PetType type, int age, string roomNumber, int days, IPricingStrategy pricingStrategy)
    {
        var room = _roomRepo.GetByNumber(roomNumber);
        
        if (room == null) 
            throw new BusinessRuleException("Кімнату не знайдено.");
            
        if (room.IsOccupied) 
            throw new BusinessRuleException("Цей вольєр уже зайнятий іншою твариною.");

        var pet = new Pet(petName, type, age);
        var booking = new Booking(pet, room, DateTime.Today, DateTime.Today.AddDays(days), pricingStrategy);

        room.MarkAsOccupied();
        _roomRepo.Update(room);
        _bookingRepo.Add(booking);

        // Асинхронний пакетний сейв змін у JSON-файли інфраструктури
        await _roomRepo.SaveChangesAsync();
        await _bookingRepo.SaveChangesAsync();

        return booking;
    }

    /// <summary>
    /// Асинхронно завершує активне проживання тварини (UC-2), переводячи його в архів та звільняючи номерний фонд.
    /// </summary>
    /// <param name="bookingId">Унікальний системний ідентифікатор бронювання (Guid).</param>
    /// <exception cref="BusinessRuleException">Викидається, якщо бронювання з таким ID не існує в системі.</exception>
    public async Task CompleteBookingAsync(Guid bookingId)
    {
        var bookings = _bookingRepo.GetAll();
        var booking = bookings.FirstOrDefault(b => b.Id == bookingId) 
            ?? throw new BusinessRuleException("Бронювання не знайдено.");

        booking.CompleteBooking();
        _roomRepo.Update(booking.Room);

        await _roomRepo.SaveChangesAsync();
        await _bookingRepo.SaveChangesAsync();
    }

    /// <summary>
    /// Асинхронно скасовує активне бронювання тварини (UC-3) за ініціативою клієнта з миттєвим звільненням вольєра.
    /// </summary>
    /// <param name="bookingId">Унікальний системний ідентифікатор бронювання (Guid).</param>
    /// <exception cref="BusinessRuleException">Викидається, якщо бронювання не знайдено або вже не є активним.</exception>
    public async Task CancelBookingAsync(Guid bookingId)
    {
        var bookings = _bookingRepo.GetAll();
        var booking = bookings.FirstOrDefault(b => b.Id == bookingId) 
            ?? throw new BusinessRuleException("Бронювання не знайдено.");

        booking.CancelBooking();
        _roomRepo.Update(booking.Room);

        await _roomRepo.SaveChangesAsync();
        await _bookingRepo.SaveChangesAsync();
    }
}