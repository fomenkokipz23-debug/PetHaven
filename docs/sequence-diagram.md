sequenceDiagram
    actor Admin as Адміністратор (Console)
    participant Service as BookingService (Application)
    participant RoomRepo as RoomRepository (Infrastructure)
    participant Booking as Booking (Domain)
    participant BookingRepo as BookingRepository (Infrastructure)

    Admin->>Service: BookRoom(pet, roomNumber, days)
    Service->>RoomRepo: GetByNumber(roomNumber)
    RoomRepo-->>Service: Room Object
    
    Service->>Booking: new Booking(pet, room, checkIn, checkOut)
    Note over Booking: Перевірка інваріантів<br/>та розрахунок вартості
    Booking-->>Service: Booking Object
    
    Service->>RoomRepo: Update(room.MarkAsOccupied())
    Service->>BookingRepo: Add(booking)
    
    Service-->>Admin: Booking Details (Success)