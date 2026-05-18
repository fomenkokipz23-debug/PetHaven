using PetHaven.Domain;

namespace PetHaven.Application;

public class BookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;

    public BookingService(IBookingRepository bookingRepository, IRoomRepository roomRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
    }

    public Booking BookRoom(string petName, PetType type, int age, string roomNumber, int days)
    {
        var room = _roomRepository.GetByNumber(roomNumber);
        if (room == null) throw new Exception("Кімнату не знайдено.");
        if (room.IsOccupied) throw new Exception("Кімната вже зайнята.");

        var pet = new Pet(petName, type, age);
        
        DateTime checkIn = DateTime.Today;
        DateTime checkOut = checkIn.AddDays(days);

        var booking = new Booking(pet, room, checkIn, checkOut);
        
        room.MarkAsOccupied();
        _roomRepository.Update(room);
        _bookingRepository.Add(booking);

        return booking;
    }
}