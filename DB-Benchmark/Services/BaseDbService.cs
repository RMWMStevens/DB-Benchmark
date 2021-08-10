using DB_Benchmark.Helpers;
using DB_Benchmark.Models.Enums;
using System;
using System.Threading.Tasks;

namespace DB_Benchmark.Services
{
    public abstract class BaseDbService
    {
        private string connectionString;
        public string ConnectionString { get => connectionString; }

        public abstract DatabaseSystem System { get; }

        public static string GetSettingsFilePath(DatabaseSystem system)
        {
            return $"./DB-Benchmark - Settings/mssql-to-mongodb_{system}.txt";
        }

        public void ShowConnectionInfo()
        {
            Console.WriteLine($"Current {System} connection string: \n{connectionString}");
        }

        public void ShowSetConnectionStringMessage(int menuIndex)
        {
            Console.WriteLine($"{menuIndex}) Set {System} connection string");
        }

        public async Task LoadConfigFromFileSystemAsync()
        {
            LogHelper.Log(message: $"Loading {System} configuration file...");

            var loadResult = await FileHelper.LoadAsync<string>(GetSettingsFilePath(System));
            if (!loadResult.IsSuccess)
            {
                LogHelper.Log(loadResult.Message + "\n");
                return;
            }

            LogHelper.Log($"Read {System} connection strings successfully from local filesystem\n");
            connectionString = loadResult.Data;
        }

        public async Task SetConnectionString()
        {
            try
            {
                Console.WriteLine($"Setting connection string for database system: {System}");
                Console.WriteLine($"Leave empty and press Enter to skip setting a new string\n");
                Console.WriteLine($"The connection string should be of the following format: \n{GetExampleConnectionStringFormat()}\n\n");
                Console.WriteLine("Enter your connecting string below:");

                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) { return; }

                connectionString = input;
                await FileHelper.SaveAsync(GetSettingsFilePath(System), connectionString);
            }
            catch (Exception ex)
            {
                LogHelper.Log($"Setting new connection string went wrong: {ex.Message}");
            }
        }

        public abstract string GetExampleConnectionStringFormat();
    }
}
