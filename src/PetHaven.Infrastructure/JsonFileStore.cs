using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PetHaven.Infrastructure;

public class JsonFileStore<T>
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options;

    public JsonFileStore(string fileName)
    {
        string docsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
        if (!Directory.Exists(docsPath)) Directory.CreateDirectory(docsPath);
        _filePath = Path.Combine(docsPath, fileName);

        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles // Захист від зациклень сутностей
        };
    }

    public async Task SaveAsync(IEnumerable<T> data)
    {
        try
        {
            using var stream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
            await JsonSerializer.SerializeAsync(stream, data, _options);
        }
        catch (IOException ex)
        {
            throw new Exception($"Помилка доступу до файлу при збереженні: {ex.Message}", ex);
        }
    }

    public async Task<List<T>> LoadAsync()
    {
        if (!File.Exists(_filePath)) return new List<T>();

        try
        {
            using var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
            var result = await JsonSerializer.DeserializeAsync<List<T>>(stream, _options);
            return result ?? new List<T>();
        }
        catch (JsonException)
        {
            // Якщо файл пошкоджений, повертаємо порожній список (або робимо бекап у реальному житті)
            return new List<T>();
        }
    }
}