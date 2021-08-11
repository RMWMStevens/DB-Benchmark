using System;

namespace DB_Benchmark.Helpers
{
    public static class LogHelper
    {
        static bool isEnabled = true;

        public static void Enable() { isEnabled = true; }

        public static void Disable() { isEnabled = false; }

        public static void Log(string message, string location = "")
        {
            if (!isEnabled) { return; }

            var log = $"[{GetTimestamp()}]";
            if (!string.IsNullOrEmpty(location)) { log += $" {location} |"; }
            log += $" {message}";

            Console.WriteLine(log);
        }

        public static void LogError(string message, string location = "")
        {
            if (!isEnabled) { return; }

            Console.ForegroundColor = ConsoleColor.Red;
            Log(message, location);
            Console.ResetColor();
        }

        private static string GetTimestamp() => DateTime.Now.ToString("HH:mm:ss.fff");
    }
}
