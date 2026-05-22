using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // ОБОВ'ЯЗКОВО ДОДАТИ ЦЕ
using PetHaven.Domain;

namespace PetHaven.Infrastructure;

public class InMemoryRoomRepository : IRoomRepository
{
    private readonly List<Room> _rooms = new();

    public void Add(Room room) => _rooms.Add(room);
    public Room GetByNumber(string number) => 
        _rooms.FirstOrDefault(r => r.Number.Equals(number, StringComparison.OrdinalIgnoreCase))!;
    public IEnumerable<Room> GetAvailableRooms(RoomType type) => 
        _rooms.Where(r => r.Category == type && !r.IsOccupied);
    public void Update(Room room)
    {
        var index = _rooms.FindIndex(r => r.Id == room.Id);
        if (index != -1) _rooms[index] = room;
    }

    // ДОДАТИ ЦЕЙ МЕТОД ДЛЯ ВИКОНАННЯ КОНТРАКТУ
    public Task SaveChangesAsync() => Task.CompletedTask;
}

public class InMemoryBookingRepository : IBookingRepository
{
    private readonly List<Booking> _bookings = new();

    public void Add(Booking booking) => _bookings.Add(booking);
    public IEnumerable<Booking> GetAll() => _bookings;

    // ДОДАТИ ЦЕЙ МЕТОД ДЛЯ ВИКОНАННЯ КОНТРАКТУ
    public Task SaveChangesAsync() => Task.CompletedTask;
}