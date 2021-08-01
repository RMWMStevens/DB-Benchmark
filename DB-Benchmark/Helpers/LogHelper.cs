﻿using System;

namespace DB_Benchmark.Helpers
{
    public static class LogHelper
    {
        public static void Log(string message, string location = "")
        {
            var log = $"[{GetTimestamp()}]";
            if (!string.IsNullOrEmpty(location)) { log += $" {location} |"; }
            log += $" {message}";

            Console.WriteLine(log);
        }

        public static void Warn(string message, string location = "")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Log(message, location);
            Console.ResetColor();
        }

        private static string GetTimestamp()
        {
            return DateTime.Now.ToString("HH:mm:ss.fff");
        }
    }
}