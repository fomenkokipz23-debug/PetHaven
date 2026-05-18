namespace PetHaven.Domain;

public class Booking
{
    public Guid Id { get; } = Guid.NewGuid();
    public Pet Pet { get; }
    public Room Room { get; }
    public DateTime CheckInDate { get; }
    public DateTime CheckOutDate { get; }
    public decimal TotalPrice { get; }

    public Booking(Pet pet, Room room, DateTime checkIn, DateTime checkOut)
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
        TotalPrice = CalculatePrice();
    }

    private decimal CalculatePrice()
    {
        int days = (CheckOutDate - CheckInDate).Days;
        return days * Room.PricePerNight;
    }
}