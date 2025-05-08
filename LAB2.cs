using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public interface ICrudServiceAsync<T> : IEnumerable<T>
{
    Task<bool> CreateAsync(T element);
    Task<T> ReadAsync(Guid id);
    Task<IEnumerable<T>> ReadAllAsync();
    Task<IEnumerable<T>> ReadAllAsync(int page, int amount);
    Task<bool> UpdateAsync(T element);
    Task<bool> RemoveAsync(T element);
    Task<bool> SaveAsync();
}

public class ThreadSafeCrudService<T> : ICrudServiceAsync<T> where T : IEntity
{
    private readonly ConcurrentDictionary<Guid, T> _storage = new();
    private readonly string _filePath;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly AutoResetEvent _saveEvent = new(false);

    public ThreadSafeCrudService(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<bool> CreateAsync(T element)
    {
        return await Task.FromResult(_storage.TryAdd(element.Id, element));
    }

    public async Task<T> ReadAsync(Guid id)
    {
        _storage.TryGetValue(id, out var result);
        return await Task.FromResult(result);
    }

    public async Task<IEnumerable<T>> ReadAllAsync()
    {
        return await Task.FromResult(_storage.Values);
    }

    public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        return await Task.FromResult(_storage.Values.Skip((page - 1) * amount).Take(amount));
    }

    public async Task<bool> UpdateAsync(T element)
    {
        if (_storage.ContainsKey(element.Id))
        {
            _storage[element.Id] = element;
            return await Task.FromResult(true);
        }
        return await Task.FromResult(false);
    }

    public async Task<bool> RemoveAsync(T element)
    {
        return await Task.FromResult(_storage.TryRemove(element.Id, out _));
    }

    public async Task<bool> SaveAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(_storage.Values, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
            _saveEvent.Set();
            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public IEnumerator<T> GetEnumerator() => _storage.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public interface IEntity
{
    Guid Id { get; }
}

public class Bus : IEntity
{
    private static readonly Random _random = new();
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Capacity { get; set; }

    public static Bus CreateNew()
    {
        return new Bus
        {
            Id = Guid.NewGuid(),
            Name = $"Bus_{Guid.NewGuid().ToString().Substring(0, 5)}",
            Capacity = _random.Next(30, 60)
        };
    }
}

class Program
{
    static async Task Main()
    {
        string filePath = "buses.json";
        var service = new ThreadSafeCrudService<Bus>(filePath);

        int count = 200;
        var tasks = new List<Task>();
        object lockObj = new();

        Parallel.For(0, count, i =>
        {
            var bus = Bus.CreateNew();
            lock (lockObj)
            {
                tasks.Add(service.CreateAsync(bus));
            }
        });

        await Task.WhenAll(tasks);

        Console.WriteLine("Збереження автобусів у файл...");
        await service.SaveAsync();
        Console.WriteLine("Файл успішно збережено!");

        var allBuses = await service.ReadAllAsync();
        var capacities = allBuses.Select(b => b.Capacity);

        int min = capacities.Min();
        int max = capacities.Max();
        double avg = capacities.Average();

        Console.WriteLine($"Мінімальна місткість: {min}");
        Console.WriteLine($"Максимальна місткість: {max}");
        Console.WriteLine($"Середня місткість: {avg:F2}");

        Console.WriteLine("Очікуємо сигнал збереження...");
        await service.SaveAsync();
        Console.WriteLine("Файл знову збережено.");
    }
}
