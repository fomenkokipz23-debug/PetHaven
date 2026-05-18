using System;
using PetHaven.Application;
using PetHaven.Domain;
using PetHaven.Infrastructure;

var roomRepo = new InMemoryRoomRepository();
roomRepo.Add(new Room("101", RoomType.Standard, 350));
roomRepo.Add(new Room("102", RoomType.Comfort, 500));
roomRepo.Add(new Room("201", RoomType.Luxury, 800));

var bookingRepo = new InMemoryBookingRepository();
var bookingService = new BookingService(bookingRepo, roomRepo);

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=== Вітаємо в системі PetHaven ===");

while (true)
{
    Console.WriteLine("\nОберіть дію:");
    Console.WriteLine("1. Переглянути вільні кімнати (Comfort)");
    Console.WriteLine("2. Заселити тварину (Вертикальний зріз)");
    Console.WriteLine("3. Вихід");
    Console.Write("> ");
    
    var choice = Console.ReadLine();

    if (choice == "1")
    {
        var rooms = roomRepo.GetAvailableRooms(RoomType.Comfort);
        foreach (var r in rooms)
            Console.WriteLine($"Кімната №{r.Number} | Категорія: {r.Category} | Ціна: {r.PricePerNight} грн/ніч");
    }
    else if (choice == "2")
    {
        try
        {
            Console.Write("Введіть ім'я тварини: ");
            string name = Console.ReadLine()!;

            Console.Write("Тип тварини (0 - Собака, 1 - Кіт, 2 - Папуга): ");
            PetType type = (PetType)int.Parse(Console.ReadLine()!);

            Console.Write("Вік тварини: ");
            int age = int.Parse(Console.ReadLine()!);

            Console.Write("Оберіть номер кімнати (напр. 101, 102, 201): ");
            string roomNum = Console.ReadLine()!;

            Console.Write("Кількість днів проживання: ");
            int days = int.Parse(Console.ReadLine()!);

            var booking = bookingService.BookRoom(name, type, age, roomNum, days);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n[Успіх] Бронювання створено!");
            Console.WriteLine($"Тварина: {booking.Pet.Name}, Кімната: {booking.Room.Number}");
            Console.WriteLine($"Загальна вартість за {days} днів: {booking.TotalPrice} грн.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Помилка] {ex.Message}");
            Console.ResetColor();
        }
    }
    else if (choice == "3") break;
}