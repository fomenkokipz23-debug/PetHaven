# PetHaven — Асинхронна система керування готелем для тварин (v1.0.0)

**PetHaven** - це високонавантажений консольний застосунок, розроблений для повної автоматизації бізнес-процесів сучасного готелю для домашніх тварин (облік постояльців, бронювання номерів різних категорій, управління файловими сховищами даних та динамічний розрахунок вартості послуг). 

---

## Архітектура та Модель даних

Проєкт розділено на 4 класичні ізольовані шари, що забезпечує гнучкість, чистоту коду та незалежність від зовнішніх змін:

### 1. PetHaven.Domain (Ядро системи)
Містить сутності домену, типи даних та чисті правила бізнес-логіки:
* `Pet` - модель тварини (ім'я, тип, вік).
* `Room` - модель готельного номера з методами контролю стану (`MarkAsOccupied()`, `Release()`).
* `Booking` - центральний агрегат, що інкапсулює логіку періоду проживання та метод розрахунку ціни `CalculatePrice()`.

###  2. PetHaven.Application
Шар сценаріїв використання (Use Cases). Головний сервіс `BookingService` координує процеси, оркеструє сутності домену та взаємодіє з абстракціями репозиторіїв (`IBookingRepository`, `IRoomRepository`) за принципом **DIP (Dependency Inversion)**.

### 3. PetHaven.Infrastructure
Реалізує збереження даних програми:
* `InMemoryRepositories` - швидкі ін-меморі сховища для сесійного тестування.
* `JsonFileStore` / `FileBookingRepository` - персистентне сховище для тривалого збереження даних у форматі JSON/CSV.

### 4. PetHaven.Console (Presentation)
Точка входу в застосунок (`Program.cs`). Інтерактивне меню для адміністратора готелю.

---

## Реалізовані архітектурні діаграми

###  Діаграма класів доменного сервісу (Class Diagram)
```mermaid
classDiagram
    direction TB
    
    class Pet {
        +Guid Id
        +string Name
        +PetType Type
        +int Age
        +Pet(string name, PetType type, int age)
    }

    class Room {
        +Guid Id
        +string Number
        +RoomType Category
        +decimal PricePerNight
        +bool IsOccupied
        +Room(string number, RoomType category, decimal price)
        +MarkAsOccupied()
        +Release()
    }

    class Booking {
        +Guid Id
        +Pet Pet
        +Room Room
        +DateTime CheckInDate
        +DateTime CheckOutDate
        +decimal TotalPrice
        +Booking(Pet pet, Room room, DateTime checkIn, DateTime checkOut)
        +CalculatePrice() decimal
    }

    class IBookingRepository {
        <<interface>>
        +Add(Booking booking)
        +GetAll() IEnumerable~Booking~
    }

    class IRoomRepository {
        <<interface>>
        +GetAvailableRooms(RoomType type) IEnumerable~Room~
        +GetByNumber(string number) Room
        +Update(Room room)
    }

    class BookingService {
        -IBookingRepository _bookingRepo
        -IRoomRepository _roomRepo
        +BookingService(IBookingRepository bRepo, IRoomRepository rRepo)
        +BookRoom(Pet pet, string roomNumber, int days) Booking
    }

    BookingService --> IBookingRepository
    BookingService --> IRoomRepository
    Booking --> Pet
    Booking --> Room