using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        var teamService = new CrudService<FootballTeam>();

        var team1 = new FootballTeam { Name = "Барселона", Country = "Іспанія", FoundedYear = 1899, StadiumCapacity = 99354 };
        var team2 = new FootballTeam { Name = "Манчестер Юнайтед", Country = "Англія", FoundedYear = 1878, StadiumCapacity = 74879 };

        teamService.Create(team1);
        teamService.Create(team2);

        Console.WriteLine("Список футбольних команд:");
        foreach (var team in teamService.ReadAll())
        {
            Console.WriteLine($"{team.Name} - {team.Country}, Заснована: {team.FoundedYear}, Місткість стадіону: {team.StadiumCapacity} місць");
        }

        Console.WriteLine("\nПошук команди за ID першої команди:");
        var foundTeam = teamService.Read(team1.Id);
        if (foundTeam != null)
        {
            Console.WriteLine($"Знайдено: {foundTeam.Name} - {foundTeam.Country}, Заснована: {foundTeam.FoundedYear}, Місткість стадіону: {foundTeam.StadiumCapacity} місць");
        }

        team1.Name = "Реал Мадрид";
        teamService.Update(team1);

        Console.WriteLine("\nОновлений список команд:");
        foreach (var team in teamService.ReadAll())
        {
            Console.WriteLine($"{team.Name} - {team.Country}, Заснована: {team.FoundedYear}, Місткість стадіону: {team.StadiumCapacity} місць");
        }

        teamService.Remove(team2);
        Console.WriteLine("\nСписок команд після видалення однієї:");
        foreach (var team in teamService.ReadAll())
        {
            Console.WriteLine($"{team.Name} - {team.Country}, Заснована: {team.FoundedYear}, Місткість стадіону: {team.StadiumCapacity} місць");
        }

        Console.WriteLine("\nПрограма завершена. Натисніть Enter для виходу...");
        Console.ReadLine();
    }
}

// Клас футбольної команди
class FootballTeam
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Country { get; set; }
    public int FoundedYear { get; set; }
    public int StadiumCapacity { get; set; }
}

// Генерік CRUD-сервіс
class CrudService<T> where T : class
{
    private readonly List<T> _items = new();

    public void Create(T element) => _items.Add(element);
    public T Read(Guid id) => _items.FirstOrDefault(e => (e as dynamic).Id == id);
    public IEnumerable<T> ReadAll() => _items;
    public void Update(T element)
    {
        var existing = Read((element as dynamic).Id);
        if (existing != null)
        {
            _items.Remove(existing);
            _items.Add(element);
        }
    }
    public void Remove(T element) => _items.Remove(element);
}