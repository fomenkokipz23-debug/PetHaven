namespace PetHaven.Domain;

public interface IRoomRepository
{
    IEnumerable<Room> GetAvailableRooms(RoomType type);
    Room GetByNumber(string number);
    void Update(Room room);
    void Add(Room room); 
}

public interface IBookingRepository
{
    void Add(Booking booking);
    IEnumerable<Booking> GetAll();
}