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