using System;
using System.Collections.Generic;
using System.Linq;
using PetHaven.Domain;

namespace PetHaven.Application;

public class AnalyticsService
{
    private readonly IBookingRepository _bookingRepo; // Використовуємо інтерфейс замість конкретного класу для дотримання SOLID

    public AnalyticsService(IBookingRepository bookingRepo)
    {
        _bookingRepo = bookingRepo;
    }

    // 1. LINQ: Список активних бронювань
    public IEnumerable<Booking> GetActiveBookings() =>
        _bookingRepo.GetAll().Where(b => b.Status == BookingStatus.Active);

    // 2. LINQ + Агрегація: Загальний прибуток готелю
    public decimal GetTotalRevenue() =>
        _bookingRepo.GetAll()
                    .Where(b => b.Status == BookingStatus.Completed || b.Status == BookingStatus.Active)
                    .Sum(b => b.TotalPrice);

    // 3. LINQ + Спеціалізована колекція (Dictionary): Групування та підрахунок тварин за типами
    public Dictionary<PetType, int> GetPetCountByType() =>
        _bookingRepo.GetAll()
                    .Where(b => b.Status == BookingStatus.Active)
                    .GroupBy(b => b.Pet.Type)
                    .ToDictionary(g => g.Key, g => g.Count());

    // 4. LINQ: Топ найдовших бронювань (сортування та обмеження вибірки)
    public IEnumerable<Booking> GetTopLongestStays(int topCount) =>
        _bookingRepo.GetAll()
                    .OrderByDescending(b => (b.CheckOutDate - b.CheckInDate).Days)
                    .Take(topCount);
}