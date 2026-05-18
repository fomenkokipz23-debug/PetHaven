using System;
using System.Threading.Tasks;
using PetHaven.Application;
using PetHaven.Domain;
using PetHaven.Infrastructure;

namespace PetHaven.ConsoleApp;

class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Ініціалізація шару інфраструктури
        var roomRepo = new FileRoomRepository();
        var bookingRepo = new FileBookingRepository();

        await roomRepo.InitializeAsync();
        await bookingRepo.InitializeAsync();

        var bookingService = new BookingService(bookingRepo, roomRepo);
        var analyticsService = new AnalyticsService(bookingRepo);

        while (true)
        {
            Console.WriteLine("\n=== КЕРУВАННЯ ГОТЕЛЕМ PETHAVEN (Ітерація 2) ===");
            Console.WriteLine("1. Заселити тварину (Check-In)");
            Console.WriteLine("2. Виселити тварину (Check-Out)");
            Console.WriteLine("3. Скасувати бронювання");
            Console.WriteLine("4. Меню аналітики та LINQ-запитів");
            Console.WriteLine("5. Вихід");
            Console.Write("> ");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                try
                {
                    Console.Write("Ім'я тварини: ");
                    string name = Console.ReadLine()!;
                    Console.Write("Тип (0 - Собака, 1 - Кіт, 2 - Папуга): ");
                    PetType type = (PetType)int.Parse(Console.ReadLine()!);
                    Console.Write("Вік: ");
                    int age = int.Parse(Console.ReadLine()!);
                    Console.Write("Номер кімнати (101, 102, 201): ");
                    string roomNum = Console.ReadLine()!;
                    Console.Write("Кількість днів: ");
                    int days = int.Parse(Console.ReadLine()!);

                    Console.WriteLine("Оберіть тариф: 1 - Стандартний, 2 - З урахуванням типу тварини");
                    int tariffChoice = int.Parse(Console.ReadLine()!);
                    IPricingStrategy strategy = tariffChoice == 2 
                        ? new AnimalSpecificPricingStrategy() 
                        : new RegularPricingStrategy();

                    var b = await bookingService.BookRoomAsync(name, type, age, roomNum, days, strategy);
                    Console.WriteLine($"[Успіх] Заселено! Розраховано за тарифом '{strategy.Name}'. Сума: {b.TotalPrice} грн.");
                }
                catch (Exception ex) { Console.WriteLine($"[Помилка] {ex.Message}"); }
            }
            else if (choice == "2")
            {
                Console.Write("Введіть ID бронювання для виселення: ");
                if (Guid.TryParse(Console.ReadLine(), out Guid id))
                {
                    try {
                        await bookingService.CompleteBookingAsync(id);
                        Console.WriteLine("[Успіх] Кімнату звільнено, статус оновлено.");
                    } catch (Exception ex) { Console.WriteLine($"[Помилка] {ex.Message}"); }
                }
            }
            else if (choice == "3")
            {
                Console.Write("Введіть ID для скасування: ");
                if (Guid.TryParse(Console.ReadLine(), out Guid id))
                {
                    try {
                        await bookingService.CancelBookingAsync(id);
                        Console.WriteLine("[Успіх] Бронювання скасовано.");
                    } catch (Exception ex) { Console.WriteLine($"[Помилка] {ex.Message}"); }
                }
            }
            else if (choice == "4")
            {
                Console.WriteLine("\n--- АНАЛІТИКА ---");
                Console.WriteLine($"Загальний дохід готелю: {analyticsService.GetTotalRevenue()} грн.");
                
                Console.WriteLine("\nАктивні постояльці:");
                foreach (var b in analyticsService.GetActiveBookings())
                    Console.WriteLine($"- ID: {b.Id} | Кімната: {b.Room.Number} | Тварина: {b.Pet.Name} ({b.Pet.Type})");

                Console.WriteLine("\nКількість активних тварин за типами:");
                foreach (var kvp in analyticsService.GetPetCountByType())
                    Console.WriteLine($"- {kvp.Key}: {kvp.Value}");
            }
            else if (choice == "5") break;
        }
    }
}