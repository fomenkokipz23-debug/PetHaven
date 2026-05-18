namespace PetHaven.Domain;

public class Room
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Number { get; }
    public RoomType Category { get; }
    public decimal PricePerNight { get; }
    public bool IsOccupied { get; private set; }

    public Room(string number, RoomType category, decimal pricePerNight)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Номер кімнати обов'язковий.");
        if (pricePerNight <= 0)
            throw new ArgumentException("Ціна має бути більшою за 0.");

        Number = number;
        Category = category;
        PricePerNight = pricePerNight;
        IsOccupied = false;
    }

    public void MarkAsOccupied() => IsOccupied = true;
    public void Release() => IsOccupied = false;
}