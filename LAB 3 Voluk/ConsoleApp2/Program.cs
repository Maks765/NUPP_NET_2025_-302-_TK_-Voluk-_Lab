using Microsoft.EntityFrameworkCore;

var options = new DbContextOptionsBuilder<TransportContext>()
    .UseSqlite("Data Source=transport.db")
    .Options;

using var context = new TransportContext(options);
var busRepository = new Repository<BusModel>(context);
var busService = new CrudServiceAsync<BusModel>(busRepository, context);

// Створення нового автобуса
await busService.CreateAsync(new BusModel
{
    LicensePlate = "AB1234CD",
    Capacity = 50,
    DriverId = 1
});

// Отримання всіх автобусів
var allBuses = await busService.ReadAllAsync();
foreach (var bus in allBuses)
{
    Console.WriteLine($"Bus {bus.Id}: {bus.LicensePlate}, Capacity: {bus.Capacity}");
}