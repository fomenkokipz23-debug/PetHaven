using System.Collections.Generic;
using System.Threading.Tasks; // Обов'язково додано для Task

namespace PetHaven.Domain;

public interface IRoomRepository
{
    IEnumerable<Room> GetAvailableRooms(RoomType type);
    Room GetByNumber(string number);
    void Update(Room room);
    void Add(Room room);
    Task SaveChangesAsync(); // Оце додано!
}

public interface IBookingRepository
{
    void Add(Booking booking);
    IEnumerable<Booking> GetAll();
    Task SaveChangesAsync(); // Оце додано!
}