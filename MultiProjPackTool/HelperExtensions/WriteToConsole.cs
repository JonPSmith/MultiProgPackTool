// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace MultiProjPackTool.HelperExtensions
{
    public interface IWriteToConsole
    {
        LogLevel DefaultLogLevel { get; set; }
        int NumWarnings { get; }
        void LogMessage(string message, LogLevel level, bool warningDoesNotStop = false);
        void OutputErrorIfAnyWarnings();
    }

    public class WriteToConsole : IWriteToConsole
    {
        public LogLevel DefaultLogLevel { get; set; } = LogLevel.Information;

        public int NumWarnings { get; private set; }

        public void LogMessage(string message, LogLevel level, bool warningDoesNotStop = false)
        {
            if (level == LogLevel.Warning && !warningDoesNotStop)
                NumWarnings++;

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

        public void OutputErrorIfAnyWarnings()
        {
            if (NumWarnings > 0)
                LogMessage($"There were {NumWarnings} warnings. The process will not continue.", LogLevel.Error);
        }

        private ConsoleColor GetColorForLevel(LogLevel level)
        {
            return (level > LogLevel.Information) ? ConsoleColor.Red : 
                level < LogLevel.Information ? ConsoleColor.Gray :  ConsoleColor.White;
        }
    }
}