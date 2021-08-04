using DB_Benchmark.Helpers;
using DB_Benchmark.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DB_Benchmark
{
    class Program
    {
        static TestRunnerService testService;

        static void Main()
        {
            testService = new TestRunnerService();

            if (testService.DbServices.Count < 1)
            {
                LogHelper.LogError("No database systems loaded. Please check the dbServices variable in the TestRunnerService.cs");
                return;
            }

            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            await LoadConfigFromFileSystemAsync();

            bool showMenu = true;
            while (showMenu)
            {
                showMenu = await ShowMenuAsync();
            }
        }

        static async Task<bool> ShowMenuAsync()
        {
            const int menuIndexOffset = 1; // Index of the first dynamically allocated database service

            // Show Menu
            Console.Clear();
            Console.WriteLine("Select an option:");
            Console.WriteLine("i) Show current connection info");
            Console.WriteLine("Enter) Start benchmarks");
            //Console.WriteLine("0) Start benchmarks without warmup");

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
                case ConsoleKey.I:
                    ShowAllConnectionInfo();
                    PressToContinue();
                    return true;
                case ConsoleKey.Enter:
                    var testSuiteAR = await testService.RunTestSuite(runWarmupTests: true);

                    if (!testSuiteAR.IsSuccess)
                    {
                        LogHelper.LogError("Something went wrong during the run: ");
                        LogHelper.LogError(testSuiteAR.Message);
                    }

                    PressToContinue();
                    return true;
                case ConsoleKey.D0:
                    var testSuiteWithoutWarmupAR = await testService.RunTestSuite(runWarmupTests: false);

                    if (!testSuiteWithoutWarmupAR.IsSuccess)
                    {
                        LogHelper.LogError("Something went wrong during the run: ");
                        LogHelper.LogError(testSuiteWithoutWarmupAR.Message);
                    }

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
