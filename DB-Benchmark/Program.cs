using DB_Benchmark.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DB_Benchmark
{
    class Program
    {
        static TestService testService;

        static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            testService = new TestService();
            await LoadConfigFromFileSystemAsync();

            bool showMenu = true;
            while (showMenu)
            {
                showMenu = await ShowMenuAsync();
            }
        }

        static async Task<bool> ShowMenuAsync()
        {
            const int menuIndexOffset = 3;

            // Show Menu
            Console.Clear();
            Console.WriteLine("Select an option:");
            Console.WriteLine("1) Start benchmarks");
            Console.WriteLine("2) Show current connection info");

            var menuListIndex = menuIndexOffset;
            foreach (var dbService in testService.DbServices)
            {
                dbService.ShowSetConnectionStringMessage(menuListIndex++);
            }

            Console.WriteLine("ESC) Exit");

            // Handle menu option choice
            var key = Console.ReadKey().Key;
            Console.Clear();

            switch (key)
            {
                case ConsoleKey.D1:
                    await testService.RunTestSuite();
                    PressToContinue();
                    return true;
                case ConsoleKey.D2:
                    ShowAllConnectionInfo();
                    PressToContinue();
                    return true;
                case ConsoleKey.Escape:
                    return false;
            }

            for (int i = 0; i < testService.DbServices.Count; i++)
            {
                if (key == (ConsoleKey)Enum.Parse(typeof(ConsoleKey), $"D{i + menuIndexOffset}"))
                {
                    await testService.DbServices[i].SetConnectionString();
                    return true;
                }
            }

            return true;
        }

        static async Task LoadConfigFromFileSystemAsync()
        {
            var loadTasks = new List<Task>();

            foreach (var dbService in testService.DbServices)
            {
                loadTasks.Add(dbService.LoadConfigFromFileSystemAsync());
            }

            await Task.WhenAll(loadTasks);

            PressToContinue();
        }

        static void ShowAllConnectionInfo()
        {
            foreach (var dbService in testService.DbServices)
            {
                dbService.ShowConnectionInfo();
                Console.WriteLine();
            }
        }

        static void PressToContinue()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
