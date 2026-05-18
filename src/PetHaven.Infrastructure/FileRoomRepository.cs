using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PetHaven.Domain;

namespace PetHaven.Infrastructure;

public class FileRoomRepository : IRoomRepository
{
    private readonly List<Room> _rooms = new();
    private readonly JsonFileStore<Room> _fileStore = new("rooms.json");

    public async Task InitializeAsync()
    {
        var data = await _fileStore.LoadAsync();
        _rooms.Clear();
        _rooms.AddRange(data);

        if (!_rooms.Any())
        {
            Add(new Room("101", RoomType.Standard, 300));
            Add(new Room("102", RoomType.Comfort, 500));
            Add(new Room("201", RoomType.Luxury, 800));
            await SaveChangesAsync();
        }
    }

    public IEnumerable<Room> GetAvailableRooms(RoomType type) => _rooms.Where(r => r.Category == type && !r.IsOccupied);
    public Room GetByNumber(string number) => _rooms.FirstOrDefault(r => r.Number == number)!;
    public void Add(Room room) => _rooms.Add(room);
    public void Update(Room room)
    {
        var existing = GetByNumber(room.Number);
        if (existing != null)
        {
            _rooms.Remove(existing);
            _rooms.Add(room);
        }
    }
    public async Task SaveChangesAsync() => await _fileStore.SaveAsync(_rooms);
}