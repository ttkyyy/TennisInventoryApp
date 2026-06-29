using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using TennisInventoryApp.Data;
using TennisInventoryApp.Data.AdoNet;
using TennisInventoryApp.Data.Ef;
using TennisInventoryApp.Models;

namespace TennisInventoryApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Загрузка конфигурации
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string connectionString = config.GetConnectionString("DefaultConnection");

            // Инициализация репозиториев
            var adoRepo = new AdoNetPlayerRepository(connectionString);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connectionString)
                .Options;
            var context = new ApplicationDbContext(options);
            var efRepo = new EfPlayerRepository(context);

            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("🏓 Управление игроками");
                Console.WriteLine("======================");
                Console.WriteLine("Выберите технологию:");
                Console.WriteLine("  1. ADO.NET");
                Console.WriteLine("  2. Entity Framework");
                Console.WriteLine("  3. Выход");
                Console.Write("> ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ShowCrudMenu("ADO.NET", adoRepo);
                        break;
                    case "2":
                        await ShowCrudMenu("Entity Framework", efRepo);
                        break;
                    case "3":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static async Task ShowCrudMenu(string techName, object repo)
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine($"📌 {techName} - CRUD операции");
                Console.WriteLine("================================");
                Console.WriteLine("  1. Показать всех игроков");
                Console.WriteLine("  2. Добавить игрока");
                Console.WriteLine("  3. Обновить игрока");
                Console.WriteLine("  4. Удалить игрока");
                Console.WriteLine("  5. Назад");
                Console.Write("> ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ShowAllPlayers(repo);
                        break;
                    case "2":
                        await AddPlayer(repo);
                        break;
                    case "3":
                        await UpdatePlayer(repo);
                        break;
                    case "4":
                        await DeletePlayer(repo);
                        break;
                    case "5":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static async Task ShowAllPlayers(object repo)
        {
            Console.Clear();
            Console.WriteLine("📋 Список игроков");
            Console.WriteLine("=================");

            try
            {
                if (repo is AdoNetPlayerRepository adoRepo)
                {
                    var players = await adoRepo.GetAllAsync();
                    foreach (var p in players)
                    {
                        Console.WriteLine($"{p.FullName} | Рейтинг: {p.Rating:F2} | {p.Email} | {(p.IsActive ? "Активен" : "Неактивен")}");
                    }
                }
                else if (repo is EfPlayerRepository efRepo)
                {
                    var players = await efRepo.GetAllAsync();
                    foreach (var p in players)
                    {
                        Console.WriteLine($"{p.FullName} | Рейтинг: {p.Rating:F2} | {p.Email} | {(p.IsActive ? "Активен" : "Неактивен")}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }

        static async Task AddPlayer(object repo)
        {
            Console.Clear();
            Console.WriteLine("➕ Добавление игрока");
            Console.WriteLine("====================");

            try
            {
                var player = new Player
                {
                    Id = Guid.NewGuid(),
                    RegisteredAt = DateTime.Now,
                    IsActive = true
                };

                Console.Write("ФИО: ");
                player.FullName = Console.ReadLine();

                Console.Write("Email: ");
                player.Email = Console.ReadLine();

                Console.Write("Телефон: ");
                player.Phone = Console.ReadLine();

                Console.Write("Дата рождения (гггг-мм-дд): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime birthDate))
                    player.BirthDate = birthDate;

                Console.Write("Рейтинг: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal rating))
                    player.Rating = rating;

                int result = 0;
                if (repo is AdoNetPlayerRepository adoRepo)
                    result = await adoRepo.AddAsync(player);
                else if (repo is EfPlayerRepository efRepo)
                    result = await efRepo.AddAsync(player);

                Console.WriteLine(result > 0 ? "✅ Игрок добавлен!" : "⚠️ Ошибка добавления.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }

        static async Task UpdatePlayer(object repo)
        {
            Console.Clear();
            Console.WriteLine("📝 Обновление игрока");
            Console.WriteLine("====================");

            try
            {
                Console.Write("Введите ID игрока: ");
                if (!Guid.TryParse(Console.ReadLine(), out Guid id))
                {
                    Console.WriteLine("❌ Неверный ID.");
                    Console.ReadKey();
                    return;
                }

                Player player = null;
                if (repo is AdoNetPlayerRepository adoRepo)
                    player = await adoRepo.GetByIdAsync(id);
                else if (repo is EfPlayerRepository efRepo)
                    player = await efRepo.GetByIdAsync(id);

                if (player == null)
                {
                    Console.WriteLine("❌ Игрок не найден.");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"Текущее имя: {player.FullName}");
                Console.Write("Новое имя (Enter - пропустить): ");
                string newName = Console.ReadLine();
                if (!string.IsNullOrEmpty(newName)) player.FullName = newName;

                Console.WriteLine($"Текущий рейтинг: {player.Rating:F2}");
                Console.Write("Новый рейтинг (Enter - пропустить): ");
                if (decimal.TryParse(Console.ReadLine(), out decimal newRating))
                    player.Rating = newRating;

                int result = 0;
                if (repo is AdoNetPlayerRepository adoRepo2)
                    result = await adoRepo2.UpdateAsync(player);
                else if (repo is EfPlayerRepository efRepo2)
                    result = await efRepo2.UpdateAsync(player);

                Console.WriteLine(result > 0 ? "✅ Игрок обновлен!" : "⚠️ Ошибка обновления.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }

        static async Task DeletePlayer(object repo)
        {
            Console.Clear();
            Console.WriteLine("🗑️ Удаление игрока");
            Console.WriteLine("==================");

            try
            {
                Console.Write("Введите ID игрока: ");
                if (!Guid.TryParse(Console.ReadLine(), out Guid id))
                {
                    Console.WriteLine("❌ Неверный ID.");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Удалить? (y/n): ");
                if (Console.ReadLine()?.ToLower() != "y")
                {
                    Console.WriteLine("🚫 Отменено.");
                    Console.ReadKey();
                    return;
                }

                int result = 0;
                if (repo is AdoNetPlayerRepository adoRepo)
                    result = await adoRepo.DeleteAsync(id);
                else if (repo is EfPlayerRepository efRepo)
                    result = await efRepo.DeleteAsync(id);

                Console.WriteLine(result > 0 ? "✅ Игрок удален!" : "⚠️ Игрок не найден.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}