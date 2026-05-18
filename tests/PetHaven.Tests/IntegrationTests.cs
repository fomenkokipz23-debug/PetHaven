using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using PetHaven.Application;
using PetHaven.Domain;
using PetHaven.Infrastructure;

namespace PetHaven.Tests;

public class IntegrationTests : IDisposable
{
    private readonly string _tempRoomFile;
    private readonly string _tempBookingFile;

    public IntegrationTests()
    {
        // Використовуємо унікальні тимчасові файли для кожного тесту
        _tempRoomFile = Path.GetTempFileName();
        _tempBookingFile = Path.GetTempFileName();
    }

    [Fact]
    public async Task FullCycle_SaveAndRestore_PreservesDataAndState()
    {
        // 1. Ініціалізація першого екземпляра системи
        var roomStore = new JsonFileStore<Room>(_tempRoomFile);
        var bookingStore = new JsonFileStore<Booking>(_tempBookingFile);

        var roomList = new List<Room> { new Room("301", RoomType.Luxury, 1000) };
        await roomStore.SaveAsync(roomList);

        // Симулюємо репозиторії
        var rooms = await roomStore.LoadAsync();
        Assert.Single(rooms);
        Assert.Equal("301", rooms.First().Number);
    }

    [Fact]
    public async Task LoadAsync_FileIsCorrupted_ReturnsEmptyCollection_FaultHandling()
    {
        // Записуємо зламаний некоректний JSON у файл
        await File.WriteAllTextAsync(_tempRoomFile, "{ invalid json ... [}");

        var roomStore = new JsonFileStore<Room>(_tempRoomFile);

        // Перевіряємо fault handling: сервіс не падає з критичною помилкою, а повертає пусту колекцію
        var rooms = await roomStore.LoadAsync();
        
        Assert.Empty(rooms);
    }

    public void Dispose()
    {
        // Очищаємо дисковий простір після тестів
        if (File.Exists(_tempRoomFile)) File.Delete(_tempRoomFile);
        if (File.Exists(_tempBookingFile)) File.Delete(_tempBookingFile);
    }
}