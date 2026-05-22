using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PetHaven.Domain;

namespace PetHaven.Infrastructure;

public class FileBookingRepository : IBookingRepository
{
    private readonly List<Booking> _bookings = new();
    private readonly JsonFileStore<Booking> _fileStore = new("bookings.json");

    public async Task InitializeAsync()
    {
        var data = await _fileStore.LoadAsync();
        _bookings.Clear();
        _bookings.AddRange(data);
    }

    public void Add(Booking booking) => _bookings.Add(booking);
    public IEnumerable<Booking> GetAll() => _bookings;
    public async Task SaveChangesAsync() => await _fileStore.SaveAsync(_bookings);
}