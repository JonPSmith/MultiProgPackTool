// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace MultProjPackTool.HelperExtensions
{
    public class ConsoleOutput
    {
        public LogLevel DefaultLogLevel { get; set; } = LogLevel.Information;

        public void LogMessage(string message, LogLevel level)
        {
            if (level >= DefaultLogLevel)
            {
                var originalColor = Console.ForegroundColor;

                Console.ForegroundColor = GetColorForLevel(level);
                if (level >= LogLevel.Warning)
                    message = $"{level.ToString().ToUpper()}: {message}";
                Console.WriteLine(message);
                Console.ForegroundColor = originalColor;
            }

            if (level >= LogLevel.Error)
                Environment.Exit(1);
        }

        private ConsoleColor GetColorForLevel(LogLevel level)
        {
            return (level > LogLevel.Information) ? ConsoleColor.Red : 
                level < LogLevel.Information ? ConsoleColor.Gray :  ConsoleColor.White;
        }
    }
}